using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace WikiUpload
{
    public class AboutViewModel : BaseViewModel
    {
        private readonly Window _window;

        public AboutViewModel(Window window)
        {
            _window = window;
            Assembly assembly = Assembly.GetEntryAssembly();
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            CopyrightText = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            VersionText = $"Version {Utils.GetApplicationVersion(assembly)}-rc1";
            CloseCommand = new RelayCommand(() => _window.Close());
        }

        public string CopyrightText { get; }

        public string VersionText { get; }

        public ICommand CloseCommand { get; set; }
    }

}
