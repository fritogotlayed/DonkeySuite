using System.Net;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Requests
{
    public class WebRequestFactoryTests
    {
        [Test]
        public void WebRequestFactory_CreateWebRequest_CreatesANewWebRequestEachCall()
        {
            // Act
            var request1 = WebRequestFactory.CreateWebRequest("http://localhost");
            var request2 = WebRequestFactory.CreateWebRequest("http://localhost");

            // Assert
            Assert.IsNotNull(request1);
            Assert.IsNotNull(request2);
            Assert.AreNotSame(request1, request2);
        }

        [Test]
        public void WebRequestFactory_CreateWebRequest_ReturnsMockWebRequestWhenOneIsSet()
        {
            // Arrange
            var mockWebRequest = new Mock<HttpWebRequest>();
            WebRequestFactory.AddWebRequestMock(mockWebRequest.Object);

            // Act
            var request = WebRequestFactory.CreateWebRequest("http://localhost");

            // Assert
            Assert.IsNotNull(request);
            Assert.AreSame(mockWebRequest.Object, request);
        }
    }
}