using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public class RequestProvider : IRequestProvider
    {
        private readonly SettingsManager _settingsManager;
        private readonly IWebRequestFactory _webRequestFactory;

        public RequestProvider(SettingsManager settingsManager, IWebRequestFactory webRequestFactory)
        {
            _settingsManager = settingsManager;
            _webRequestFactory = webRequestFactory;
        }

        public IAddImageRequest ProvideNewAddImageRequest(string fileName, byte[] imageBytes)
        {
            var settings = _settingsManager.GetSettings();
            return new AddImageRequest(_webRequestFactory)
            {
                RequestUrl = settings.ImageServer.ServerUrl,
                FileBytes = imageBytes,
                FileName = fileName
            };
        }
    }
}