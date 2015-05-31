using System;
using System.IO;
using System.Net;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Requests
{
    [TestFixture]
    public class BaseRequestTests
    {
        private class BaseRequestTestBundle
        {
            public BaseRequest BaseRequest { get; private set; }

            public BaseRequestTestBundle()
            {
                BaseRequest = new AddImageRequest {FileBytes = new byte[] {1, 2, 3}, FileName = "Test.foo", RequestUrl = "http://google.com/"};
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void BaseRequest_Post_SuccessfulReturnsTrue()
        {
            // Arrange
            var testBundle = new BaseRequestTestBundle();
            var mockWebRequest = new Mock<WebRequest>(MockBehavior.Strict);
            var mockResponse = new Mock<HttpWebResponse>(MockBehavior.Strict);

            mockWebRequest.SetupSet(x => x.Method = "POST");
            mockWebRequest.SetupSet(x => x.ContentType = "application/x-www-form-urlencoded");
            mockWebRequest.SetupSet(x => x.ContentLength = 30);
            mockWebRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
            mockWebRequest.Setup(x => x.GetResponse()).Returns(mockResponse.Object);
            WebRequestFactory.AddWebRequestMock(mockWebRequest.Object);

            mockResponse.Setup(x => x.StatusDescription).Returns("OK");
            mockResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            mockResponse.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            // Act
            var result = testBundle.BaseRequest.Post();

            // Assert
            Assert.AreEqual(true, result);
        }

        [Test]
        public void BaseRequest_Post_NotSuccessfulReturnsFalse()
        {
            // Arrange
            var testBundle = new BaseRequestTestBundle();
            var mockWebRequest = new Mock<WebRequest>(MockBehavior.Strict);
            var mockResponse = new Mock<HttpWebResponse>(MockBehavior.Strict);

            mockWebRequest.SetupSet(x => x.Method = "POST");
            mockWebRequest.SetupSet(x => x.ContentType = "application/x-www-form-urlencoded");
            mockWebRequest.SetupSet(x => x.ContentLength = 30);
            mockWebRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
            mockWebRequest.Setup(x => x.GetResponse()).Returns(mockResponse.Object);
            WebRequestFactory.AddWebRequestMock(mockWebRequest.Object);

            mockResponse.Setup(x => x.StatusDescription).Returns("BadRequest");
            mockResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadRequest);
            mockResponse.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            // Act
            var result = testBundle.BaseRequest.Post();

            // Assert
            Assert.AreEqual(false, result);
        }
    }
}