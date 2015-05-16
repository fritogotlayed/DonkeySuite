using System.Threading;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public class SemaphoreWrapper : ISemaphoreWrapper
    {
        private readonly Semaphore _semaphore;
        public SemaphoreWrapper(int initialCount, int maximumCount)
        {
            _semaphore = new Semaphore(initialCount, maximumCount);
        }

        public void WaitOne()
        {
            _semaphore.WaitOne();
        }

        public void Release()
        {
            _semaphore.Release();
        }
    }
}