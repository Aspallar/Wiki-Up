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
            var version = assembly.GetName().Version;
            return $"{version.Major}.{version.Minor}.{version.Build}";
        }
    }
}
