using System;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServerTest
    {
        [SetUp]
        public void SetUp()
        {
            DependencyManager.Kernel = null;
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void ImageServerInitializedNull()
        {
            // Act
            var server = new ImageServer();

            // Assert
            Assert.AreEqual(null, server.ServerUrl);
        }

        [Test]
        public void ImageServerConfiguredProperly()
        {
            // Arrange
            var server = new ImageServer();

            // Act
            server.PopulateWithDefaults();

            // Assert
            Assert.AreEqual("http://localhost:8080/DonkeyImageServer", server.ServerUrl);
        }

        [Test]
        public void ImageServerSetsWorkCorrectly()
        {
            // Arrange
            var server = new ImageServer();
            var url = "http://localhost:9000/Blah";

            // Act
            server.ServerUrl = url;

            // Assert
            Assert.AreEqual(url, server.ServerUrl);
        }

    }
}