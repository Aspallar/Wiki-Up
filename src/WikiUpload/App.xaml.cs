using System;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows;
using WikiUpload.Properties;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    internal partial class App : Application
    {
        public static Skin Skin { get; private set; }

        public App(): base()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // This needs to be done in the constructor as we need Skin set before the
            // app.xaml resource dictionaries are loaded.
            if (Settings.Default.FirstRun)
                PerformFirstRunActions();
            Skin = (Skin)WikiUpload.Properties.Settings.Default.Theme;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var language = Settings.Default.Language;
            if (!string.IsNullOrEmpty(language) && language != "Default")
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);

            base.OnStartup(e);

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            GetCommandLineArguments(e.Args, out var timeout);
            Timewout = timeout;
        }

        private static void PerformFirstRunActions()
        {
            try
            {
                var configurationUpgrade = new ConfigurationUpgrade(new Helpers());
                configurationUpgrade.UpgradePreviousConfiguration(new string[] { PasswordStore.StoreName });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"{nameof(PerformFirstRunActions)} in App.xaml.cs failed.\n {ex}");
                // Fail silently
            }
            Settings.Default.Reload();
            Settings.Default.FirstRun = false;
            Settings.Default.Save();
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
