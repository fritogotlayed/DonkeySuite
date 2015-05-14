namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServer
    {
        public string ServerUrl { get; set; }

        public void PopulateWithDefaults() {
            ServerUrl = "http://localhost:8080/DonkeyImageServer";
        }
    }
}