using System.Collections.Generic;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public interface IWatchDirectories : IList<WatchDirectory>
    {
        void PopulateWithDefaults();
    }
}