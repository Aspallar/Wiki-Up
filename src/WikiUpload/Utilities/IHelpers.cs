using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IHelpers
    {
        Process LaunchProcess(string path);
        string ReadAllText(string path);
        Task Wait(int ms);
        Task Wait(int ms, CancellationToken token);
        void WriteAllText(string path, string content);
    }
}