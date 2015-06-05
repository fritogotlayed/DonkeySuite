using NUnit.Framework;
using MadDonkeySoftware.SystemWrappers.Net;
using Moq;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Requests
{
    public class WebRequestFactoryTests
    {
        [Test]
        public void WebRequestFactory_CreateWebRequest_CreatesANewWebRequestEachCall()
        {
            // Arrange
            var factory = new WebRequestFactory();

            // Act
            var request1 = factory.Create("http://localhost");
            var request2 = factory.Create("http://localhost");

            // Assert
            Assert.IsNotNull(request1);
            Assert.IsNotNull(request2);
            Assert.AreNotSame(request1, request2);
        }
    }
}