using System.Threading.Tasks;

namespace WikiUpload
{
    public class Delay : IDelay
    {
        public Task Wait(int ms)
        {
            return Task.Delay(ms);
        }
    }
}
