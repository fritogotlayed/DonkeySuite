namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class Settings
    {
        public ImageServer ImageServer { get; set; }
        public WatchDirectories Directories { get; set; }

        public void PopulateWithDefaults()
        {
            ImageServer = new ImageServer();
            ImageServer.PopulateWithDefaults();
            Directories = new WatchDirectories();
            Directories.PopulateWithDefaults();
        }
    }
}