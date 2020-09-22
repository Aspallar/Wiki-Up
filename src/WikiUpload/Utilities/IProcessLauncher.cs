using System.Diagnostics;

namespace WikiUpload
{
    public interface IProcessLauncher
    {
        Process Launch(string path);
    }
}