using System.Diagnostics;

namespace WikiUpload
{
    public class ProcessLauncher : IProcessLauncher
    {
        public Process Launch(string path) => Process.Start(path);
    }
}
