using System.Xml.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    [XmlRoot("settings")]
    public class SettingsRoot
    {
        private readonly IServiceLocator _serviceLocator;

        public SettingsRoot() : this(null)
        {
            // This constructor implemented so that serialization is still allowed.
        }

        public SettingsRoot(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public ImageServer ImageServer { get; set; }

        [XmlArray("WatchDirectories")]
        [XmlArrayItem("dir")]
        public WatchDirectories Directories { get; set; }

        public void PopulateWithDefaults()
        {
            ImageServer = _serviceLocator.ProvideDefaultImageServer(); // new ImageServer();
            ImageServer.PopulateWithDefaults();
            Directories = _serviceLocator.ProvideDefaultWatchDirectories(); // new WatchDirectories();
            Directories.PopulateWithDefaults();
        }
    }
}