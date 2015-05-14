namespace DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies
{
    public interface ISortStrategy
    {
        string NewFileName(string baseDirectory, string fileName);
    }
}