using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class WatchDirectories : List<WatchDirectory>
    {
        private readonly IServiceLocator _serviceLocator;

        public WatchDirectories() : this(null)
        {
        }

        public WatchDirectories(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public virtual void PopulateWithDefaults()
        {
            var d = _serviceLocator.ProvideDefaultWatchDirectory();
            d.PopulateWithDefaults();
            Add(d);
        }
    }
}