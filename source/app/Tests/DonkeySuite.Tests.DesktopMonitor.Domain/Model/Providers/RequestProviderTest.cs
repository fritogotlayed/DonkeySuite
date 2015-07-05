/*
<copyright file="RequestProviderTest.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using MadDonkeySoftware.SystemWrappers.Net;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Providers
{
    [TestFixture]
    public class RequestProviderTest
    {
        private class RequestProviderTestBundle
        {
            private RequestProvider _requestProvider;

            public Mock<IWebRequestFactory> MockWebRequestFactory { get; private set; }
            public Mock<ILogProvider> MockLogProvider { get; private set; }
            public Mock<ICredentialRepository> MockRequestCredentialProvider { get; private set; }

            public RequestProvider RequestProvider
            {
                get { return _requestProvider ?? (_requestProvider = new RequestProvider(MockWebRequestFactory.Object, MockLogProvider.Object, MockRequestCredentialProvider.Object)); }
            }

            public RequestProviderTestBundle()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                MockLogProvider = new Mock<ILogProvider>();
                MockRequestCredentialProvider = new Mock<ICredentialRepository>();
            }
        }

        [Test]
        public void RequestProviderCreatesObjectForProvidedType()
        {
            // Arrange
            var mockServer = new Mock<IImageServer>();
            var testBundle = new RequestProviderTestBundle();
            var fileBytes = new byte[5];

            mockServer.SetupGet(x => x.ServerUrl).Returns("http://localhost");

            // Act
            var req = testBundle.RequestProvider.ProvideNewAddImageRequest(mockServer.Object, "fileName.txt", fileBytes);

            // Assert
            Assert.IsNotNull(req);
            Assert.AreEqual("http://localhost/image", req.RequestUrl);
            Assert.AreEqual("fileName.txt", req.FileName);
            Assert.AreSame(fileBytes, req.FileBytes);
        }
    }
}