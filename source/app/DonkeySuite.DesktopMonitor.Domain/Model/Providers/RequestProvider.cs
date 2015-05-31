using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public class RequestProvider : IRequestProvider
    {
        private readonly SettingsManager _settingsManager;

        public RequestProvider(SettingsManager settingsManager)
        {
            _settingsManager = settingsManager;
        }

        public AddImageRequest ProvideNewAddImageRequest(string fileName, byte[] imageBytes)
        {
            var settings = _settingsManager.GetSettings();
            return new AddImageRequest()
            {
                RequestUrl = settings.ImageServer.ServerUrl,
                FileBytes = imageBytes,
                FileName = fileName
            };
        }
    }
}