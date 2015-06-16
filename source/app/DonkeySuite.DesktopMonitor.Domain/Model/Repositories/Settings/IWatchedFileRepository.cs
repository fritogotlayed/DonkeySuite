namespace DonkeySuite.DesktopMonitor.Domain.Model.Repositories.Settings
{
    public interface IWatchedFileRepository
    {
        IWatchedFile CreateNew();
        IWatchedFile LoadFileForPath(string filePath);
        void Save(IWatchedFile image);
    }
}