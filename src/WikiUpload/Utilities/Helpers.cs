using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    public class Helpers : IHelpers
    {
        public string ReadAllText(string path) => File.ReadAllText(path);

        public void WriteAllText(string path, string content) => File.WriteAllText(path, content);

        public Task Wait(int ms) => Task.Delay(ms);

        public Task Wait(int ms, CancellationToken token) => Task.Delay(ms, token);

        public Process LaunchProcess(string path) => Process.Start(path);

        public bool IsCancellationRequested(CancellationToken token) => token.IsCancellationRequested;

        public string ApplicationVersion => Utils.ApplicationVersion;
    }
}
