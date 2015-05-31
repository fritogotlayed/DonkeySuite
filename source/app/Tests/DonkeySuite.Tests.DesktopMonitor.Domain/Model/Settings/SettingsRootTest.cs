using System;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class SettingsRootTest
    {
        private class SettingsRootTestBundle
        {
            public Mock<IServiceLocator> MockServiceLocator { get; private set; }
            public SettingsRoot SettingsRoot { get; private set; }

            public SettingsRootTestBundle()
            {
                MockServiceLocator = new Mock<IServiceLocator>();
                SettingsRoot = new SettingsRoot(MockServiceLocator.Object);
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void SettingsInitializedNull()
        {
            // Act
            var testBundle = new SettingsRootTestBundle();

            // Assert
            Assert.IsNull(testBundle.SettingsRoot.ImageServer);
            Assert.IsNull(testBundle.SettingsRoot.Directories);
        }

        [Test]
        public void SettingsConfiguredProperly()
        {
            // Arrange
            var testBundle = new SettingsRootTestBundle();
            var mockImageServer = new Mock<ImageServer>();
            var mockWatchDirectories = new Mock<WatchDirectories>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServer()).Returns(mockImageServer.Object);
            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultWatchDirectories()).Returns(mockWatchDirectories.Object);

            // Act
            testBundle.SettingsRoot.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(testBundle.SettingsRoot.ImageServer);
            Assert.IsNotNull(testBundle.SettingsRoot.Directories);
        }

        [Test]
        public void SettingsSetsWorkCorrectly()
        {
            // Arrange
            var testBundle = new SettingsRootTestBundle();

            var server = new Mock<ImageServer>();
            var directories = new Mock<WatchDirectories>();

            // Act
            testBundle.SettingsRoot.ImageServer = server.Object;
            testBundle.SettingsRoot.Directories = directories.Object;

            // Assert
            Assert.AreSame(server.Object, testBundle.SettingsRoot.ImageServer);
            Assert.AreSame(directories.Object, testBundle.SettingsRoot.Directories);
        }
    }
}