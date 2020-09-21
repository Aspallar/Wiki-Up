using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IDelay
    {
        Task Wait(int ms);
    }
}