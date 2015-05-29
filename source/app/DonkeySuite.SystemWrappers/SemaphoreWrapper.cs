using System.Threading;
using DonkeySuite.SystemWrappers.Interfaces;

namespace DonkeySuite.SystemWrappers
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