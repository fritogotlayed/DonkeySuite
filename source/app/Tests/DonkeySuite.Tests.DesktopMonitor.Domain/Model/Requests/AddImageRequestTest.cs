using System;
using System.IO;
using System.Net;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using log4net;
using Moq;
using NUnit.Framework;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Requests
{
    public class AddImageRequestTest
    {
        private class AddImageRequestTestBundle
        {
            private AddImageRequest _addImageRequest;
            public Mock<IWebRequestFactory> MockWebRequestFactory { get; private set; }
            public Mock<ILogProvider> MockLogProvider { get; private set; }
            public Mock<ICredentialRepository> MockRequestCredentialProvider { get; private set; }

            public AddImageRequest AddImageRequest
            {
                get { return _addImageRequest ?? (_addImageRequest = new AddImageRequest(MockWebRequestFactory.Object, MockLogProvider.Object, MockRequestCredentialProvider.Object)); }
            }

            public AddImageRequestTestBundle()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                MockLogProvider = new Mock<ILogProvider>();
                MockRequestCredentialProvider = new Mock<ICredentialRepository>();
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        // TODO: 
        [Test]
        public void AddImageRequest_SetRequestUrl_PathWithoutTrailingSlashProperlyFormed()
        {
            // Arrange
            var testBundle = new AddImageRequestTestBundle();

            // Act
            testBundle.AddImageRequest.RequestUrl = "http://google.com";

            // Assert
            Assert.AreEqual("http://google.com/image", testBundle.AddImageRequest.RequestUrl);
        }

        [Test]
        public void AddImageRequest_SetRequestUrl_PathWithTrailingSlashProperlyFormed()
        {
            // Arrange
            var testBundle = new AddImageRequestTestBundle();

            // Act
            testBundle.AddImageRequest.RequestUrl = "http://google.com/";

            // Assert
            Assert.AreEqual("http://google.com/image", testBundle.AddImageRequest.RequestUrl);
        }

        [Test]
        public void post_CallsPopulateRequestParameters()
        {
            // Arrange
            var testBundle = new AddImageRequestTestBundle();
            var mockWebRequest = new Mock<IWebRequest>(MockBehavior.Strict);
            var mockLog = new Mock<ILog>();

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);
            testBundle.AddImageRequest.FileBytes = new byte[] {1, 2, 3};
            testBundle.AddImageRequest.FileName = "Test.foo";
            testBundle.AddImageRequest.RequestUrl = "http://google.com/";

            mockWebRequest.SetupSet(x => x.Method = "POST");
            mockWebRequest.SetupSet(x => x.ContentType = "application/x-www-form-urlencoded");
            mockWebRequest.SetupSet(x => x.ContentLength = 30);
            mockWebRequest.SetupGet(x => x.Headers).Returns(new WebHeaderCollection());
            mockWebRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
            mockWebRequest.Setup(x => x.GetResponse()).Returns(new Mock<IHttpWebResponse>().Object);
            testBundle.MockWebRequestFactory.Setup(x => x.Create(It.IsAny<string>())).Returns(mockWebRequest.Object);
            // WebRequestFactory.AddWebRequestMock(mockWebRequest.Object);

            // Act
            testBundle.AddImageRequest.Post();

            // Assert
            Assert.AreEqual("http://google.com/image", testBundle.AddImageRequest.RequestUrl);
        }
    }
}