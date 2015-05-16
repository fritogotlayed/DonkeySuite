using System.Xml.Serialization;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServer
    {
        [XmlAttribute("serverUrl")]
        public string ServerUrl { get; set; }

        public void PopulateWithDefaults() {
            ServerUrl = "http://localhost:8080/DonkeyImageServer";
        }
    }
}