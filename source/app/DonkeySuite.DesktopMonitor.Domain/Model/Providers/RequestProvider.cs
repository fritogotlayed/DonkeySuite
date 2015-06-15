using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public class RequestProvider : IRequestProvider
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IWebRequestFactory _webRequestFactory;
        private readonly ILogProvider _logProvider;

        public RequestProvider(ISettingsManager settingsManager, IWebRequestFactory webRequestFactory, ILogProvider logProvider)
        {
            _settingsManager = settingsManager;
            _webRequestFactory = webRequestFactory;
            _logProvider = logProvider;
        }

        public IAddImageRequest ProvideNewAddImageRequest(string fileName, byte[] imageBytes)
        {
            var settings = _settingsManager.GetSettings();
            return new AddImageRequest(_webRequestFactory, _logProvider)
            {
                RequestUrl = settings.ImageServer.ServerUrl,
                FileBytes = imageBytes,
                FileName = fileName
            };
        }
    }
}