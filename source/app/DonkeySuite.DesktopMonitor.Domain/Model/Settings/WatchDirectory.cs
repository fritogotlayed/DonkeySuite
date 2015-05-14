namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class WatchDirectory
    {
        public string Path { get; set; }
        public bool IncludeSubDirectories { get; set; }
        public string FileExtensions { get; set; }
        public OperationMode Mode { get; set; }
        public string SortStrategy { get; set; }

        public void PopulateWithDefaults()
        {
            Path = "C:\\"; // TODO: Make platform independent
            FileExtensions = "jpg,jpeg,gif,tiff";
            Mode = OperationMode.Unknown;
        }
    }
}