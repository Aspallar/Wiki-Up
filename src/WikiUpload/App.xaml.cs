using System;
using System.Net;
using System.Reflection;
using System.Windows;
using WikiUpload.Service;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            GetCommandLineArguments(e.Args, out int timeout);
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

        private void GetCommandLineArguments(string[] args, out int timeout)
        {
            timeout = 0;
            foreach (var arg in args)
            {
                if (int.TryParse(arg, out int value))
                    timeout = value;
            }
        }

        public static string UserAgent
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                AssemblyTitleAttribute title = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(
                    assembly, typeof(AssemblyTitleAttribute));
                string userAgent = $"{title.Title}/{Utils.GetApplicationVersion(assembly)}";
                return userAgent;
            }
        }

        public static int Timewout { get; private set; }

        public static INavigatorService Navigator { get; set; }
    }
}
