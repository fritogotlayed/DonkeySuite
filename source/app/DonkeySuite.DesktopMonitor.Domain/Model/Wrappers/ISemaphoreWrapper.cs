namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public interface ISemaphoreWrapper
    {
        void WaitOne();
        void Release();
    }
}