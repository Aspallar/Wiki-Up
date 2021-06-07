using System.Threading.Tasks;

namespace WikiUpload
{
    internal interface IUpdateCheck
    {
        Task<UpdateCheckResponse> CheckForUpdates(string userAgent, int delay);
    }
}