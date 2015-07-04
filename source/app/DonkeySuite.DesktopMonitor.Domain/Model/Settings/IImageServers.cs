using System.Collections.Generic;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public interface IImageServers : IList<ImageServer>
    {
        void PopulateWithDefaults();
    }
}