using System.Xml.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    [XmlRoot("settings")]
    public class SettingsRoot
    {
        private readonly IEntityProvider _entityLocator;

        public SettingsRoot() : this(null)
        {
            // This constructor implemented so that serialization is still allowed.
        }

        public SettingsRoot(IEntityProvider entityLocator)
        {
            _entityLocator = entityLocator;
        }

        public ImageServers ImageServers { get; set; }

        public void PopulateWithDefaults()
        {
            ImageServers = (ImageServers) _entityLocator.ProvideDefaultImageServers();
        }
    }
}