using System;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServerTest
    {
        private class ImageServerTestBundle
        {
            public ImageServer ImageServer { get; private set; }

            public ImageServerTestBundle()
            {
                ImageServer = new ImageServer();
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void ImageServerInitializedNull()
        {
            // Arrange
            var testBundle = new ImageServerTestBundle();

            // Act
            var url = testBundle.ImageServer.ServerUrl;

            // Assert
            Assert.AreEqual(null, url);
        }

        [Test]
        public void ImageServerConfiguredProperly()
        {
            // Arrange
            var testBundle = new ImageServerTestBundle();

            // Act
            testBundle.ImageServer.PopulateWithDefaults();

            // Assert
            Assert.AreEqual("http://localhost:8080/DonkeyImageServer", testBundle.ImageServer.ServerUrl);
        }

        [Test]
        public void ImageServerSetsWorkCorrectly()
        {
            // Arrange
            var testBundle = new ImageServerTestBundle();
            var url = "http://localhost:9000/Blah";

            // Act
            testBundle.ImageServer.ServerUrl = url;

            // Assert
            Assert.AreEqual(url, testBundle.ImageServer.ServerUrl);
        }

    }
}