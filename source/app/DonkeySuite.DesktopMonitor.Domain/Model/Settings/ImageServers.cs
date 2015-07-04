using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServers : List<ImageServer>, IImageServers
    {
        private readonly IEntityProvider _entityProvider;

        public ImageServers() : this(null)
        {
        }

        public ImageServers(IEntityProvider entityProvider)
        {
            _entityProvider = entityProvider;
        }

        public virtual void PopulateWithDefaults()
        {
            Add(_entityProvider.ProvideDefaultImageServer());
        }
    }
}