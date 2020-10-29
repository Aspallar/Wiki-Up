using System;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IUpdateCheck
    {
        event EventHandler<CheckForUpdatesEventArgs> CheckForUpdateCompleted;

        Task CheckForUpdates(string userAgent, int delay);
    }
}