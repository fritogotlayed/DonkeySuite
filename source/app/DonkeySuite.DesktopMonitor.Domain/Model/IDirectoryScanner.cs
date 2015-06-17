using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public interface IDirectoryScanner
    {
        IList<IWatchedFile> GetAvailableImages(IWatchedFileRepository watchedFileRepository, string path, IList<string> acceptableExtensions, bool includeSubDirectories,
                                               ISortStrategy sortStrategy);
    }
}