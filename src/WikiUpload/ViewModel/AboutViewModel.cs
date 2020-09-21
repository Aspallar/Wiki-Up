using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace WikiUpload
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            CopyrightText = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            VersionText = $"Version {Utils.GetApplicationVersion(assembly)}";
            CloseCommand = new RelayParameterizedCommand((window) => ((Window)window).Close());
            LaunchWebSiteCommand = new RelayCommand(() => Process.Start("https://github.com/Aspallar/Wiki-Up"));
        }

        public string CopyrightText { get; }

        public string VersionText { get; }

        public ICommand CloseCommand { get; set; }

        public ICommand LaunchWebSiteCommand { get; set; }
    }

}
