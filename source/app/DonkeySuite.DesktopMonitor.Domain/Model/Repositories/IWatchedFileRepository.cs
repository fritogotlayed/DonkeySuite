namespace DonkeySuite.DesktopMonitor.Domain.Model.Repositories
{
    public interface IWatchedFileRepository
    {
        IWatchedFile CreateNew();
        IWatchedFile LoadFileForPath(string filePath);
        void Save(IWatchedFile image);
    }
}