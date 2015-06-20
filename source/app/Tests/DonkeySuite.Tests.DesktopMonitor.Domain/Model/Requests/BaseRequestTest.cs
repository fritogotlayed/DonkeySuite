using System;
using System.Collections.Generic;
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
    [TestFixture]
    public class BaseRequestTests
    {
        private class BaseRequestTestBundle
        {
            private BaseRequest _baseRequest;
            public Mock<IWebRequestFactory> MockWebRequestFactory { get; private set; }
            public Mock<ILogProvider> MockLogProvider { get; private set; }
            public Mock<ICredentialRepository> MockRequestCredentialProvider { get; private set; }
            public BaseRequest BaseRequest
            {
                get { return _baseRequest ?? (_baseRequest = new TestRequest(MockWebRequestFactory.Object, MockLogProvider.Object, MockRequestCredentialProvider.Object)); }
            }

            private class TestRequest : BaseRequest
            {
                public TestRequest(IWebRequestFactory webRequestFactory, ILogProvider logProvider, ICredentialRepository credentialRepository) : base(webRequestFactory, logProvider, credentialRepository)
                {
                }

                protected override void PopulateRequestParameters(Dictionary<string, string> parameters)
                {
                    parameters.Add("key", "value");
                }
            }

            public BaseRequestTestBundle()
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

        [Test]
        public void BaseRequest_Post_SuccessfulReturnsTrue()
        {
            // Arrange
            var testBundle = new BaseRequestTestBundle();
            var mockWebRequest = new Mock<IWebRequest>();
            var mockResponse = new Mock<IHttpWebResponse>();
            var mockLog = new Mock<ILog>();

            testBundle.MockWebRequestFactory.Setup(x => x.Create(It.IsAny<string>())).Returns(mockWebRequest.Object);
            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);

            mockWebRequest.SetupGet(x => x.Headers).Returns(new WebHeaderCollection());
            mockWebRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
            mockWebRequest.Setup(x => x.GetResponse()).Returns(mockResponse.Object);

            mockResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.OK);
            mockResponse.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            // Act
            var result = testBundle.BaseRequest.Post();

            // Assert
            Assert.AreEqual(true, result);
            mockWebRequest.VerifySet(x => x.Method = "POST");
            mockWebRequest.VerifySet(x => x.ContentType = "application/x-www-form-urlencoded");
            mockWebRequest.VerifySet(x => x.ContentLength = 9);
        }

        [Test]
        public void BaseRequest_Post_NotSuccessfulReturnsFalse()
        {
            // Arrange
            var testBundle = new BaseRequestTestBundle();
            var mockWebRequest = new Mock<IWebRequest>();
            var mockResponse = new Mock<IHttpWebResponse>();
            var mockLog = new Mock<ILog>();

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);

            mockWebRequest.SetupGet(x => x.Headers).Returns(new WebHeaderCollection());
            mockWebRequest.Setup(x => x.GetRequestStream()).Returns(new MemoryStream());
            mockWebRequest.Setup(x => x.GetResponse()).Returns(mockResponse.Object);
            testBundle.MockWebRequestFactory.Setup(x => x.Create(It.IsAny<string>())).Returns(mockWebRequest.Object);

            mockResponse.Setup(x => x.StatusDescription).Returns("BadRequest");
            mockResponse.Setup(x => x.StatusCode).Returns(HttpStatusCode.BadRequest);
            mockResponse.Setup(x => x.GetResponseStream()).Returns(new MemoryStream());

            // Act
            var result = testBundle.BaseRequest.Post();

            // Assert
            Assert.AreEqual(false, result);
            mockWebRequest.VerifySet(x => x.Method = "POST");
            mockWebRequest.VerifySet(x => x.ContentType = "application/x-www-form-urlencoded");
            mockWebRequest.VerifySet(x => x.ContentLength = 9);
        }

        [Test]
        public void BaseRequest_Post_ExceptionThrownReturnsFalseAndLogsMessage()
        {
            // Arrange
            var testBundle = new BaseRequestTestBundle();
            var testException = new WebException("Test exception.");
            var mockLog = new Mock<ILog>();

            testBundle.MockWebRequestFactory.Setup(x => x.Create(It.IsAny<string>())).Throws(testException);
            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);

            // Act
            var result = testBundle.BaseRequest.Post();

            // Assert
            Assert.AreEqual(false, result);
            const string message = "Failed posting to server.";
            mockLog.Verify(x => x.Error(It.Is<string>(y => y.StartsWith(message)), It.IsAny<WebException>()), Times.Once);
        }
    }
}