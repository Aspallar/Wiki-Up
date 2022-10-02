using System.Threading;

namespace Tests
{
    public class SynchonousSynchronizationContext : SynchronizationContext
    {
        public override void Post(SendOrPostCallback d, object state)
        {
            d.Invoke(state);
        }
        
        public override void Send(SendOrPostCallback d, object state)
        {
            d.Invoke(state); 
        }
    }
}
