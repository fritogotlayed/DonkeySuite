using System.Threading;

namespace MadDonkeySoftware.SystemWrappers.Threading
{
    public class SemaphoreWrapper : ISemaphore
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