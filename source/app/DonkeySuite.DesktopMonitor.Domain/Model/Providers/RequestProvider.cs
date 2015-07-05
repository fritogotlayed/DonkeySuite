/*
<copyright file="RequestProvider.cs">
   Copyright 2015 MadDonkey Software

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
</copyright>
*/
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