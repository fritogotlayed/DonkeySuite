using System.IO;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public interface IEntityProvider
    {
        TextWriter ProvideTextWriter(string path);
        ISortStrategy ProvideDefaultSortStrategy();
        ISortStrategy ProvideSortStrategy(string key);
        SettingsRoot ProvideDefaultSettingsRoot();
        WatchDirectory ProvideDefaultWatchDirectory();
        WatchDirectories ProvideDefaultWatchDirectories();
        ImageServer ProvideDefaultImageServer();
    }
}