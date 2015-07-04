using System;
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
            public Mock<IEntityProvider> MockServiceLocator { get; private set; }
            public SettingsRoot SettingsRoot { get; private set; }

            public SettingsRootTestBundle()
            {
                MockServiceLocator = new Mock<IEntityProvider>();
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
            Assert.IsNull(testBundle.SettingsRoot.ImageServers);
        }

        [Test]
        public void SettingsConfiguredProperly()
        {
            // Arrange
            var testBundle = new SettingsRootTestBundle();
            var mockImageServers = new Mock<ImageServers>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServers()).Returns(mockImageServers.Object);

            // Act
            testBundle.SettingsRoot.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(testBundle.SettingsRoot.ImageServers);
        }

        [Test]
        public void SettingsSetsWorkCorrectly()
        {
            // Arrange
            var testBundle = new SettingsRootTestBundle();

            var mockImageServers = new Mock<ImageServers>();

            // Act
            testBundle.SettingsRoot.ImageServers = mockImageServers.Object;

            // Assert
            Assert.AreSame(mockImageServers.Object, testBundle.SettingsRoot.ImageServers);
        }
    }
}