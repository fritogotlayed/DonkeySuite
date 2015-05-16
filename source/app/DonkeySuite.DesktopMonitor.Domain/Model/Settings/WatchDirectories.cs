using System.Collections.Generic;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class WatchDirectories : List<WatchDirectory> // IEnumerable<WatchDirectory>
    {
        public void PopulateWithDefaults()
        {
            var d = new WatchDirectory();
            d.PopulateWithDefaults();
            Add(d);
        }
    }
}