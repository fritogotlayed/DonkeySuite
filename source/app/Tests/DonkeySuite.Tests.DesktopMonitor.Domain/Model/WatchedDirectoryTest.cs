using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model
{
    [TestFixture]
    public class WatchedDirectoryTests
    {
        private class WatchedDirectoryTestBundle
        {
            private WatchedDirectory _watchedDirectory;

            public Mock<ILogProvider> MockLogProvider { get; private set; }
            public Mock<IServiceLocator> MockServiceLocator { get; private set; }
            public Mock<IDirectoryScanner> MockDirectoryScanner { get; private set; }

            public WatchedDirectory WatchedDirectory
            {
                get { return _watchedDirectory ?? (_watchedDirectory = new WatchedDirectory(MockServiceLocator.Object, MockLogProvider.Object, MockDirectoryScanner.Object)); }
            }

            public WatchedDirectoryTestBundle()
            {
                MockLogProvider = new Mock<ILogProvider>();
                MockServiceLocator = new Mock<IServiceLocator>();
                MockDirectoryScanner = new Mock<IDirectoryScanner>();
            }
        }

        [Test]
        public void WatchedDirectoryConfigureWhenStrategyNotDefinedPullsValuesFromWatchDirectoryProperly()
        {
            // Arrange
            var testBundle = new WatchedDirectoryTestBundle();
            var mockWatchDirectory = new Mock<IWatchDirectory>();

            mockWatchDirectory.SetupGet(x => x.FileExtensions).Returns("abc");

            // Act
            testBundle.WatchedDirectory.Configure(mockWatchDirectory.Object);

            // Assert
            mockWatchDirectory.VerifyGet(x => x.Path, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.IncludeSubDirectories, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.Mode, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.SortStrategy, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.FileExtensions, Times.Once());
        }

        [Test]
        public void WatchedDirectoryConfigureWhenStrategyDefinedPullsValuesFromWatchDirectoryProperly()
        {
            // Arrange
            var testBundle = new WatchedDirectoryTestBundle();
            var mockWatchDirectory = new Mock<IWatchDirectory>();

            mockWatchDirectory.SetupGet(x => x.FileExtensions).Returns("abc");
            mockWatchDirectory.SetupGet(x => x.SortStrategy).Returns("simple");

            // Act
            testBundle.WatchedDirectory.Configure(mockWatchDirectory.Object);

            // Assert
            mockWatchDirectory.VerifyGet(x => x.Path, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.IncludeSubDirectories, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.Mode, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.SortStrategy, Times.Once());
            mockWatchDirectory.VerifyGet(x => x.FileExtensions, Times.Once());
        }

        [Test]
        public void WatchedDirectoryProcessAvailableImagesWhenNothingToProcess()
        {
            // Arrange
            var testBundle = new WatchedDirectoryTestBundle();
            var mockWatchFileRepository = new Mock<IWatchedFileRepository>();

            testBundle.MockDirectoryScanner.Setup(x => x.GetAvailableImages(mockWatchFileRepository.Object, It.IsAny<string>(), It.IsAny<IList<string>>(), It.IsAny<bool>(), It.IsAny<ISortStrategy>()))
                      .Returns(new List<IWatchedFile>());

            // Act
            testBundle.WatchedDirectory.ProcessAvailableImages(mockWatchFileRepository.Object);

            // Assert
            mockWatchFileRepository.Verify(x => x.LoadFileForPath(It.IsAny<string>()), Times.Never);
            mockWatchFileRepository.Verify(x => x.CreateNew(), Times.Never);
            mockWatchFileRepository.Verify(x => x.Save(It.IsAny<WatchedFile>()), Times.Never);
        }
    }
}