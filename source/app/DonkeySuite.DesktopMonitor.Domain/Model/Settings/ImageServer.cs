using System.Xml.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServer : IImageServer
    {
        private readonly IEntityProvider _entityProvider;

        public ImageServer() : this(null)
        {
        }

        public ImageServer(IEntityProvider entityProvider)
        {
            _entityProvider = entityProvider;
        }

        [XmlAttribute("serverUrl")]
        public string ServerUrl { get; set; }

        [XmlArray("WatchDirectories")]
        [XmlArrayItem("dir")]
        public WatchDirectories Directories { get; set; }

        public virtual void PopulateWithDefaults()
        {
            ServerUrl = "http://localhost:8080/DonkeyImageServer";
            Directories = (WatchDirectories) _entityProvider.ProvideDefaultWatchDirectories();
        }
    }
}