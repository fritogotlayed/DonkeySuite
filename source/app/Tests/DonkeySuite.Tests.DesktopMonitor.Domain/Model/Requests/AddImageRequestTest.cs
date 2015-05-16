using System;
using System.IO;
using System.Net;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using Moq;
using Ninject;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Requests
{
    public class AddImageRequestTest
    {
        private IKernel _testKernel;

        [SetUp]
        public void SetUp()
        {
            _testKernel = new StandardKernel();
            DependencyManager.Kernel = _testKernel;
        }

        [TearDown]
        public void TearDown()
        {
            _testKernel.Dispose();
            GC.Collect();
        }

        [Test]
        public void AddImageRequest_SetRequestUrl_PathWithoutTrailingSlashProperlyFormed()
        {
            // Arrange
            var request = new AddImageRequest();

            // Act
            request.RequestUrl = "http://google.com";

            // Assert
            Assert.AreEqual("http://google.com/image", request.RequestUrl);
        }

        [Test]
        public void AddImageRequest_SetRequestUrl_PathWithTrailingSlashProperlyFormed()
        {
            // Arrange
            var request = new AddImageRequest();

            // Act
            request.RequestUrl = "http://google.com/";

            // Assert
            Assert.AreEqual("http://google.com/image", request.RequestUrl);
        }

        [Test]
        public void post_CallsPopulateRequestParameters()
        {
            // Arrange
            var mockWebRequest = new Mock<WebRequest>(MockBehavior.Strict);
            var request = new AddImageRequest {FileBytes = new byte[] {1, 2, 3}, FileName = "Test.foo", RequestUrl = "http://google.com/"};

            mockWebRequest.SetupSet(x => x.Method = "POST");
            mockWebRequest.SetupSet(x => x.ContentType = "application/x-www-form-urlencoded");
            mockWebRequest.SetupSet(x => x.ContentLength = 30);
            mockWebRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
            mockWebRequest.Setup(x => x.GetResponse()).Returns(new Mock<HttpWebResponse>().Object);
            WebRequestFactory.AddWebRequestMock(mockWebRequest.Object);

            // Act
            request.Post();

            // Assert
            Assert.AreEqual("http://google.com/image", request.RequestUrl);
        }
    }
}