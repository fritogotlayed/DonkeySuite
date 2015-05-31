namespace MadDonkeySoftware.SystemWrappers.Threading
{
    public interface ISemaphore
    {
        void WaitOne();
        void Release();
    }
}