using System;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IUpdateCheck
    {
        Task<UpdateCheckResponse> CheckForUpdates(string userAgent, int delay);
    }
}