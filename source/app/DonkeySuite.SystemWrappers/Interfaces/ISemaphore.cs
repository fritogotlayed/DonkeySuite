namespace DonkeySuite.SystemWrappers.Interfaces
{
    public interface ISemaphore
    {
        void WaitOne();
        void Release();
    }
}