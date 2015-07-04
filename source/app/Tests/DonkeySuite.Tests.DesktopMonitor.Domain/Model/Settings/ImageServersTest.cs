using System;
using System.Collections;
using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class ImageServersTest
    {
        private class ImageServersTestBundle
        {
            public Mock<IEntityProvider> MockServiceLocator { get; private set; }
            public ImageServers ImageServers { get; private set; }

            public ImageServersTestBundle()
            {
                MockServiceLocator = new Mock<IEntityProvider>();
                ImageServers = new ImageServers(MockServiceLocator.Object);
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void WatchDirectoriesConstructedWithNull()
        {
            // Arrange
            var watchDirectories = new ImageServers();
            NullReferenceException expectedException = null;

            try
            {
                watchDirectories.PopulateWithDefaults();
            }
            catch (NullReferenceException ex)
            {
                expectedException = ex;
            }

            Assert.IsNotNull(expectedException);
        }

        [Test]
        public void ImageServersInitializedNull()
        {
            // Arrange & Act
            var testBundle = new ImageServersTestBundle();

            // Assert
            Assert.AreEqual(new List<WatchDirectory>(), testBundle.ImageServers);
        }

        [Test]
        public void ImageServersConfiguredProperly()
        {
            // Arrange
            var testBundle = new ImageServersTestBundle();
            var mockImageServer = new Mock<ImageServer>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServer()).Returns(mockImageServer.Object);

            // Act
            testBundle.ImageServers.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(testBundle.ImageServers);
            Assert.AreEqual(1, testBundle.ImageServers.Count);
        }

        [Test]
        public void ImageServersEnumeratorIsCorrectForGeneric()
        {
            // Arrange
            var testBundle = new ImageServersTestBundle();
            var mockImageServer = new Mock<ImageServer>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServer()).Returns(mockImageServer.Object);
            testBundle.ImageServers.PopulateWithDefaults();

            // Act
            var it = testBundle.ImageServers.GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }

        [Test]
        public void ImageServersEnumeratorIsCorrectForNonGeneric()
        {
            // Arrange
            var testBundle = new ImageServersTestBundle();
            var mockImageServer = new Mock<ImageServer>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServer()).Returns(mockImageServer.Object);
            testBundle.ImageServers.PopulateWithDefaults();

            // Act
            var it = ((IEnumerable)testBundle.ImageServers).GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }
    }
}