namespace DonkeySuite.DesktopMonitor.Domain.Model.Repositories.Settings
{
    public interface IWatchedFileRepository
    {
        WatchedFile CreateNew();
        WatchedFile LoadFileForPath(string filePath);
        void Save(WatchedFile image);
    }
}