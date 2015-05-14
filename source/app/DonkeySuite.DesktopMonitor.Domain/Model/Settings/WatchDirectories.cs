using System.Collections;
using System.Collections.Generic;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class WatchDirectories : IEnumerable<WatchDirectory>
    {
        public List<WatchDirectory> Directories { get; set; }

        public void PopulateWithDefaults() {
            var d = new WatchDirectory();
            d.PopulateWithDefaults();
            Directories.Add(d);
        }

        public IEnumerator<WatchDirectory> GetEnumerator()
        {
            return Directories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}