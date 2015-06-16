namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public interface IWatchDirectory
    {
        string Path { get; set; }
        bool IncludeSubDirectories { get; set; }
        string FileExtensions { get; set; }
        OperationMode Mode { get; set; }
        string SortStrategy { get; set; }
        void PopulateWithDefaults();
    }
}