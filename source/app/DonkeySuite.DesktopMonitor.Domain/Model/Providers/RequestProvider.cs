using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
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
        private readonly ICredentialRepository _credentialRepository;

        public RequestProvider(ISettingsManager settingsManager, IWebRequestFactory webRequestFactory, ILogProvider logProvider, ICredentialRepository credentialRepository)
        {
            _settingsManager = settingsManager;
            _webRequestFactory = webRequestFactory;
            _logProvider = logProvider;
            _credentialRepository = credentialRepository;
        }

        public IAddImageRequest ProvideNewAddImageRequest(string fileName, byte[] imageBytes)
        {
            var settings = _settingsManager.GetSettings();
            return new AddImageRequest(_webRequestFactory, _logProvider, _credentialRepository)
            {
                RequestUrl = settings.ImageServer.ServerUrl,
                FileBytes = imageBytes,
                FileName = fileName
            };
        }
    }
}