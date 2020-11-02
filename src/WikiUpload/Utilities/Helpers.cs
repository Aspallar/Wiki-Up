using System.Diagnostics;
using System.IO;
using System.Reflection;
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

        public void SignalCancel(CancellationTokenSource tokenSource) => tokenSource.Cancel();

        public string ApplicationVersion => Utils.ApplicationVersion;

        public string UserAgent => App.UserAgent;

        public (string copyright, string version) ApplicationInformation
        {
            get
            {
                Assembly assembly = Assembly.GetEntryAssembly();
                object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                var copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
                var version = Utils.GetApplicationVersion(assembly);
                return (copyright, version);
            }
        }

        public string CopyrightText { get; private set; }
        public string VersionText { get; private set; }
    }
}
