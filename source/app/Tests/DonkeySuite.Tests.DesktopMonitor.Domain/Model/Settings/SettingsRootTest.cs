using System;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.Wrappers;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class SettingsRootTest
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
        public void SettingsInitializedNull()
        {
            // Act
            var settings = new SettingsRoot();

            // Assert
            Assert.AreEqual(null, settings.ImageServer);
            Assert.AreEqual(null, settings.Directories);
        }

        [Test]
        public void SettingsConfiguredProperly()
        {
            // Arrange
            var settings = new SettingsRoot();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();

            DependencyManager.Kernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);

            // Act
            settings.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(settings.ImageServer);
            Assert.IsNotNull(settings.Directories);
        }

        [Test]
        public void SettingsSetsWorkCorrectly()
        {
            // Arrange
            var settings = new SettingsRoot();
            var server = new ImageServer();
            var directories = new WatchDirectories();

            // Act
            settings.ImageServer = server;
            settings.Directories = directories;

            // Assert
            Assert.AreSame(server, settings.ImageServer);
            Assert.AreSame(directories, settings.Directories);
        }
    }
}