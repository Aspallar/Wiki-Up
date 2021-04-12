using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IHelpers
    {
        string ApplicationVersionString { get; }
        Version ApplicationVersion { get; }
        string UserAgent { get; }
        Process LaunchProcess(string path);
        string ReadAllText(string path);
        Task Wait(int ms);
        Task Wait(int ms, CancellationToken token);
        void WriteAllText(string path, string content);
        bool IsCancellationRequested(CancellationToken token);
        void SignalCancel(CancellationTokenSource tokenSource);
        (string copyright, string version) ApplicationInformation { get; }
        IEnumerable<string> EnumerateFiles(string rootPath, string pattern, SearchOption searchOption);
    }
}