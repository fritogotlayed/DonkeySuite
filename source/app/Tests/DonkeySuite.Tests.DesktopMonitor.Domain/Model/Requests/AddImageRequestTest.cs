using System;
using System.IO;
using System.Net;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using Moq;
using NUnit.Framework;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Requests
{
    public class AddImageRequestTest
    {
        private class AddImageRequestTestBundle
        {
            public Mock<IWebRequestFactory> MockWebRequestFactory { get; private set; }
            public AddImageRequest AddImageRequest { get; private set; }

            public AddImageRequestTestBundle()
            {
                MockWebRequestFactory = new Mock<IWebRequestFactory>();
                AddImageRequest = new AddImageRequest(MockWebRequestFactory.Object);
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
            testBundle.AddImageRequest.FileBytes = new byte[] {1, 2, 3};
            testBundle.AddImageRequest.FileName = "Test.foo";
            testBundle.AddImageRequest.RequestUrl = "http://google.com/";

            mockWebRequest.SetupSet(x => x.Method = "POST");
            mockWebRequest.SetupSet(x => x.ContentType = "application/x-www-form-urlencoded");
            mockWebRequest.SetupSet(x => x.ContentLength = 30);
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