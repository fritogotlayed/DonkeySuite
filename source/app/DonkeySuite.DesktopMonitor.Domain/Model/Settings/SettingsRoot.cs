using System.Xml.Serialization;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    [XmlRoot("settings")]
    public class SettingsRoot
    {
        public ImageServer ImageServer { get; set; }

        [XmlArray("WatchDirectories")]
        [XmlArrayItem("dir")]
        public WatchDirectories Directories { get; set; }

        public void PopulateWithDefaults()
        {
            ImageServer = new ImageServer();
            ImageServer.PopulateWithDefaults();
            Directories = new WatchDirectories();
            Directories.PopulateWithDefaults();
        }
    }
}