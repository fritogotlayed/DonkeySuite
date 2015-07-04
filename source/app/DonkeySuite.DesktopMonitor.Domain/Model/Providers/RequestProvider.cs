using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Providers
{
    public class RequestProvider : IRequestProvider
    {
        private readonly IWebRequestFactory _webRequestFactory;
        private readonly ILogProvider _logProvider;
        private readonly ICredentialRepository _credentialRepository;

        public RequestProvider(IWebRequestFactory webRequestFactory, ILogProvider logProvider, ICredentialRepository credentialRepository)
        {
            _webRequestFactory = webRequestFactory;
            _logProvider = logProvider;
            _credentialRepository = credentialRepository;
        }

        public IAddImageRequest ProvideNewAddImageRequest(IImageServer server, string fileName, byte[] imageBytes)
        {
            return new AddImageRequest(_webRequestFactory, _logProvider, _credentialRepository)
            {
                RequestUrl = server.ServerUrl,
                FileBytes = imageBytes,
                FileName = fileName
            };
        }
    }
}