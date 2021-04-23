using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Skin Skin { get; } = (Skin)WikiUpload.Properties.Settings.Default.Theme;

        protected override void OnStartup(StartupEventArgs e)
         {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            var language = WikiUpload.Properties.Settings.Default.Language;
            if (!string.IsNullOrEmpty(language) && language != "Default")
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
            UploadResponse.Initialize();
            base.OnStartup(e);
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GetCommandLineArguments(e.Args, out var timeout);
            Timewout = timeout;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var msg = "An unexpected error occured\r\n";
            if (e.ExceptionObject is Exception ex)
            {
                msg += ex.Message;
                if (ex.InnerException != null)
                    msg += "\r\b" + ex.InnerException.Message;
            }
            MessageBox.Show(msg, "Error");
            Environment.Exit(1);
        }

        private static void GetCommandLineArguments(string[] args, out int timeout)
        {
            timeout = 0;
            foreach (var arg in args)
            {
                if (int.TryParse(arg, out var value))
                    timeout = value;
            }
        }

        public static string UserAgent
        {
            get
            {
                var assembly = Assembly.GetEntryAssembly();
                var title = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                    assembly, typeof(AssemblyTitleAttribute));
                var userAgent = $"{title.Title}/{Utils.GetApplicationVersion(assembly)}";
                return userAgent;
            }
        }

        public static int Timewout { get; private set; }

        public static INavigatorService Navigator { get; set; }
        
        public static ServiceLocator ServiceLocator { get; set; }
    }
}
