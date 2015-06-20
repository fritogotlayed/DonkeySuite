using System;
using System.Threading;

namespace MadDonkeySoftware.SystemWrappers.Threading
{
    public class SemaphoreWrapper : ISemaphore, IDisposable
    {
        private Semaphore _semaphore;

        public SemaphoreWrapper(int initialCount, int maximumCount)
        {
            _semaphore = new Semaphore(initialCount, maximumCount);
        }

        ~SemaphoreWrapper()
        {
            Dispose(false);
        }

        public void WaitOne()
        {
            _semaphore.WaitOne();
        }

        public void Release()
        {
            _semaphore.Release();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                if (_semaphore != null)
                {
                    _semaphore.Dispose();
                    _semaphore = null;
                }
            }
        }
    }
}