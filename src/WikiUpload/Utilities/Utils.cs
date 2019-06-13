using System;
using System.Reflection;
using System.Windows;

namespace WikiUpload
{
    public static class Utils
    {
        public static string ApplicationVersion
        {
            get
            {
                return GetApplicationVersion(Assembly.GetEntryAssembly());
            }
        }

        public static string GetApplicationVersion(Assembly assembly)
        {
            Version version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }

        public static void ErrorMessage(string message)
        {
            MessageBox.Show(message, "Wiki-Up Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
