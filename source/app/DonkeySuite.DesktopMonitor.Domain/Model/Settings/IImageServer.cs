namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public interface IImageServer
    {
        string ServerUrl { get; set; }
        // IWatchDirectories Directories { get; set; }
        WatchDirectories Directories { get; set; }
        void PopulateWithDefaults();
    }
}