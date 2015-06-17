using System;
using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using log4net;
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

            testBundle.MockDirectoryScanner.Setup(x => x.GetAvailableImages(mockWatchFileRepository.Object, null, It.IsAny<IList<string>>(), false, null))
                      .Returns(new List<IWatchedFile>());

            // Act
            testBundle.WatchedDirectory.ProcessAvailableImages(mockWatchFileRepository.Object);

            // Assert
            mockWatchFileRepository.Verify(x => x.LoadFileForPath(It.IsAny<string>()), Times.Never);
            mockWatchFileRepository.Verify(x => x.CreateNew(), Times.Never);
            mockWatchFileRepository.Verify(x => x.Save(It.IsAny<WatchedFile>()), Times.Never);
        }

        [Test]
        public void WatchedDirectoryProcessAvailableImagesWhenImageHasNotBeenUploadedAndModeIsUploadOnlyShouldUploadFile()
        {
            // Arrange
            var testBundle = new WatchedDirectoryTestBundle();
            var watchDirectory = new WatchDirectory {FileExtensions = "abc", Mode = OperationMode.UploadOnly};
            var mockWatchedFileRepository = new Mock<IWatchedFileRepository>();
            var mockWatchedFile = new Mock<IWatchedFile>();

            testBundle.MockDirectoryScanner.Setup(x => x.GetAvailableImages(mockWatchedFileRepository.Object, null, It.IsAny<IList<string>>(), false, null))
                      .Returns(new List<IWatchedFile> {mockWatchedFile.Object});

            testBundle.WatchedDirectory.Configure(watchDirectory);

            // Act
            testBundle.WatchedDirectory.ProcessAvailableImages(mockWatchedFileRepository.Object);

            // Assert
            mockWatchedFileRepository.Verify(x => x.LoadFileForPath(It.IsAny<string>()), Times.Never);
            mockWatchedFileRepository.Verify(x => x.CreateNew(), Times.Never);
            mockWatchedFileRepository.Verify(x => x.Save(mockWatchedFile.Object), Times.Once);
            mockWatchedFile.VerifySet(x => x.UploadSuccessful = false, Times.Never);
            mockWatchedFile.Verify(x => x.SendToServer(), Times.Once);
            mockWatchedFile.Verify(x => x.SortFile(), Times.Never);
            mockWatchedFile.Verify(x => x.RemoveFromDisk(), Times.Never);
        }

        [Test]
        public void WatchedDirectoryProcessAvailableImagesWhenImageIsInBaseDirectoryAndModeIsSortOnlyShouldSortFile()
        {
            // Arrange
            var testBundle = new WatchedDirectoryTestBundle();
            var watchDirectory = new WatchDirectory {FileExtensions = "abc", Mode = OperationMode.SortOnly};
            var mockWatchedFileRepository = new Mock<IWatchedFileRepository>();
            var mockWatchedFile = new Mock<IWatchedFile>();
            var mockLog = new Mock<ILog>();

            testBundle.MockDirectoryScanner.Setup(x => x.GetAvailableImages(mockWatchedFileRepository.Object, null, It.IsAny<IList<string>>(), false, null))
                      .Returns(new List<IWatchedFile> {mockWatchedFile.Object});

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);

            mockWatchedFile.Setup(x => x.IsInBaseDirectory(null)).Returns(true);

            // Act
            testBundle.WatchedDirectory.Configure(watchDirectory);
            testBundle.WatchedDirectory.ProcessAvailableImages(mockWatchedFileRepository.Object);

            // Assert
            mockWatchedFileRepository.Verify(x => x.LoadFileForPath(It.IsAny<string>()), Times.Never);
            mockWatchedFileRepository.Verify(x => x.CreateNew(), Times.Never);
            mockWatchedFileRepository.Verify(x => x.Save(mockWatchedFile.Object), Times.Never);
            mockWatchedFile.VerifySet(x => x.UploadSuccessful = false, Times.Never);
            mockWatchedFile.Verify(x => x.SendToServer(), Times.Never);
            mockWatchedFile.Verify(x => x.SortFile(), Times.Once);
            mockWatchedFile.Verify(x => x.RemoveFromDisk(), Times.Never);
        }
    }
}