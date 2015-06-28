using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class WatchDirectories : List<WatchDirectory>
    {
        private readonly IEntityProvider _entityLocator;

        public WatchDirectories() : this(null)
        {
        }

        public WatchDirectories(IEntityProvider entityLocator)
        {
            _entityLocator = entityLocator;
        }

        public virtual void PopulateWithDefaults()
        {
            var d = _entityLocator.ProvideDefaultWatchDirectory();
            d.PopulateWithDefaults();
            Add(d);
        }
    }
}