using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IDelay
    {
        Task Wait(int ms);
        Task Wait(int ms, CancellationToken token);
    }
}