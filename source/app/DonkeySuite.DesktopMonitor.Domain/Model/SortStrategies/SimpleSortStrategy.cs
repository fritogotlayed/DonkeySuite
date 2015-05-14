using System.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies
{
    public class SimpleSortStrategy : ISortStrategy
    {
        public string NewFileName(string baseDirectory, string fileName)
        {
            // Pull the first letter off the file name as a directory.
            return Path.Combine(baseDirectory, fileName[0].ToString().ToLower(), fileName);
        }
    }
}