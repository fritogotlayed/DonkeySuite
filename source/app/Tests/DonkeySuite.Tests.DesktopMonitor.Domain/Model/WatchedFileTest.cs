using System;
using System.IO;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using DonkeySuite.DesktopMonitor.Domain.Model.Wrappers;
using log4net;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model
{
    [TestFixture]
    public class WatchedFileTests : TestBase
    {
        [Test]
        public void WatchedFileReturnsSetSortStrategyWhenGetSortStrategyCalled()
        {
            // Arrange
            var strategy = new SimpleSortStrategy();
            var mockLog = new Mock<ILog>();

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);

            // Act & Assert
            var watchedFile = new WatchedFile {SortStrategy = strategy};
            Assert.AreSame(strategy, watchedFile.SortStrategy);
        }

        [Test]
        public void WatchedFileReturnsDefaultSortStrategyWhenGetSortStrategyCalled()
        {
            // Arrange
            var strategy = new SimpleSortStrategy();
            var mockLog = new Mock<ILog>();

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);
            TestKernel.Bind<ISortStrategy>().ToMethod(context => strategy).Named("defaultSortStrategy");

            // Act & Assert
            var watchedFile = new WatchedFile();
            Assert.AreSame(strategy, watchedFile.SortStrategy);
        }

        [Test]
        public void WatchedFileLoadImageBytesReturnsFileDataWithInfoLogging()
        {
            // Arrange
            const string filePath = "/foo/bar/blah.txt";
            var mockLog = new Mock<ILog>();
            var mockFileWrapper = new Mock<IFileWrapper>();

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);

            mockLog.SetupGet(x => x.IsInfoEnabled).Returns(true);
            mockFileWrapper.Setup(x => x.ReadAllBytes(filePath)).Returns(new byte[] {1, 4, 3});

            // Act
            var f = new WatchedFile {FileName = "blah.txt", FullPath = filePath};
            var fileBytes = f.LoadImageBytes();

            // Assert
            Assert.IsNotNull(fileBytes);
            Assert.AreEqual(3, fileBytes.Length);
            mockLog.VerifyGet(x => x.IsInfoEnabled, Times.Exactly(2));
            mockLog.Verify(x => x.InfoFormat("Beginning LoadImage: \"{0}\"", "/foo/bar/blah.txt"), Times.Once);
            mockLog.Verify(x => x.InfoFormat("Returning LoadImage: \"{0}\"", "/foo/bar/blah.txt"), Times.Once);
        }

        [Test]
        public void WatchedFileLoadImageBytesReturnsFileDataWithoutInfoLogging()
        {
            // Arrange
            const string filePath = "/foo/bar/blah.txt";
            var mockLog = new Mock<ILog>();
            var mockFileWrapper = new Mock<IFileWrapper>();

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);

            mockLog.SetupGet(x => x.IsInfoEnabled).Returns(false);
            mockFileWrapper.Setup(x => x.ReadAllBytes(filePath)).Returns(new byte[] {1, 4, 3});

            // Act
            var f = new WatchedFile {FileName = "blah.txt", FullPath = filePath};
            var fileBytes = f.LoadImageBytes();

            // Assert
            Assert.IsNotNull(fileBytes);
            Assert.AreEqual(3, fileBytes.Length);
            mockLog.VerifyGet(x => x.IsInfoEnabled, Times.Exactly(2));
            mockLog.Verify(x => x.Debug("Beginning LoadImage method."), Times.Once);
            mockLog.Verify(x => x.Debug("Returning LoadImage result."), Times.Once);
        }

        [Test]
        public void WatchedFileSendToServerTransmitsFileAndSetsUploadSuccessfulProperly()
        {
            // Arrange
            const string filePath = "/foo/bar/testFile.txt";
            var mockLog = new Mock<ILog>();
            var mockFileWrapper = new Mock<IFileWrapper>();
            var mockSemaphoreWrapper = new Mock<ISemaphoreWrapper>();
            var mockSerializer = new Mock<ISerializer>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();
            var mockStreamWriter = new Mock<TextWriter>();
            var mockAddImageRequest = new Mock<AddImageRequest>();

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);
            TestKernel.Bind<ISemaphoreWrapper>().ToMethod(context => mockSemaphoreWrapper.Object);
            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            TestKernel.Bind<TextWriter>().ToMethod(context => mockStreamWriter.Object);
            TestKernel.Bind<AddImageRequest>().ToMethod(context => mockAddImageRequest.Object);


            mockAddImageRequest.Setup(x => x.Post()).Returns(true);
            mockLog.SetupGet(x => x.IsInfoEnabled).Returns(false);
            mockFileWrapper.Setup(x => x.ReadAllBytes(filePath)).Returns(new byte[] {1, 4, 3});

            // Act
            var file = new WatchedFile {FileName = "testFile.txt", FullPath = filePath};
            file.SendToServer();

            // Assert
            Assert.AreEqual(true, file.UploadSuccessful);
            mockAddImageRequest.VerifySet(x => x.RequestUrl = "http://localhost:8080/DonkeyImageServer");
            mockAddImageRequest.VerifySet(x => x.FileName = "testFile.txt");
            mockLog.Verify(x => x.InfoFormat("Transmitting image: {0}", "/foo/bar/testFile.txt"), Times.Once);
        }

        [Test]
        public void WatchedFileIsInBaseDirectoryReturnsTrueWhenFileIsInBaseDirectory()
        {
            // Arrange
            const string fullPath = "C:\\foo.txt";
            const string baseDirectory = "C:\\";
            const string fileName = "foo.txt";
            var mockLog = new Mock<ILog>();

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);

            var f = new WatchedFile();
            f.FileName = fileName;
            f.FullPath = fullPath;

            // Act
            var rslt = f.IsInBaseDirectory(baseDirectory);

            // Assert
            Assert.AreEqual(true, rslt);
        }

        [Test]
        public void WatchedFileIsInBaseDirectoryReturnsFalseWhenFileIsInBaseDirectory()
        {
            // Arrange
            const string fullPath = "C:\\bar\\foo.txt";
            const string baseDirectory = "C:\\";
            const string fileName = "foo.txt";
            var mockLog = new Mock<ILog>();

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);

            var f = new WatchedFile();
            f.FileName = fileName;
            f.FullPath = fullPath;

            // Act
            var rslt = f.IsInBaseDirectory(baseDirectory);

            // Assert
            Assert.AreEqual(false, rslt);
        }

        [Test]
        public void WatchedFileSortFileThrowsExceptionWhenNoDefaultSortStrategyDefined()
        {
            // Arrange
            var mockLogger = new Mock<ILog>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();

            mockEnvironmentWrapper.SetupGet(x => x.DirectorySeparatorChar).Returns('/');

            TestKernel.Bind<ILog>().ToMethod(context => mockLogger.Object);
            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);

            // Act
            var file = new WatchedFile
            {
                FullPath = "/foo/bar.txt",
                FileName = "bar.txt",
                UploadSuccessful = true
            };

            try
            {
                file.SortFile();
            }
            catch (InvalidOperationException ex)
            {
                Assert.AreEqual("Sort strategy is not set.", ex.Message);
            }
        }

        [Test]
        public void WatchedFileSortFileLogsMessageWhenDestinationAlreadyHasFileWithSameName()
        {
            // Arrange
            var mockLog = new Mock<ILog>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();
            var mockSortStrategy = new Mock<ISortStrategy>();
            var mockFileWrapper = new Mock<IFileWrapper>();

            mockEnvironmentWrapper.SetupGet(x => x.DirectorySeparatorChar).Returns('/');

            mockSortStrategy.Setup(x => x.NewFileName("/foo", "bar.txt")).Returns("/foo/b/bar.txt");

            mockFileWrapper.Setup(x => x.Exists("/foo/b/bar.txt")).Returns(true);

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);
            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);

            var file = new WatchedFile
            {
                FullPath = "/foo/bar.txt",
                FileName = "bar.txt",
                SortStrategy = mockSortStrategy.Object,
                UploadSuccessful = true
            };

            // Act
            file.SortFile();

            // Assert
            mockLog.Verify(x => x.InfoFormat("Moving file failed due to existing file in destination. File name: {0}", "bar.txt"), Times.Once);
        }

        [Test]
        public void WatchedFileSortFileMovesFileBasedOnSortStrategyWhenDestinationIsClear()
        {
            // Arrange
            var mockLog = new Mock<ILog>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();
            var mockSortStrategy = new Mock<ISortStrategy>();
            var mockFileWrapper = new Mock<IFileWrapper>();
            var mockDirectoryWrapper = new Mock<IDirectoryWrapper>();

            mockEnvironmentWrapper.SetupGet(x => x.DirectorySeparatorChar).Returns('/');

            mockSortStrategy.Setup(x => x.NewFileName("/foo", "bar.txt")).Returns("/foo/b/bar.txt");

            mockFileWrapper.Setup(x => x.Exists("/foo/b/bar.txt")).Returns(false);

            TestKernel.Bind<ILog>().ToMethod(context => mockLog.Object);
            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);
            TestKernel.Bind<IDirectoryWrapper>().ToMethod(context => mockDirectoryWrapper.Object);

            var file = new WatchedFile
            {
                FullPath = "/foo/bar.txt",
                FileName = "bar.txt",
                SortStrategy = mockSortStrategy.Object,
                UploadSuccessful = true
            };

            // Act
            file.SortFile();

            // Assert
            mockLog.Verify(x => x.InfoFormat("Renaming file. From: {0} To: {1}", "/foo/bar.txt", "/foo/b/bar.txt"), Times.Once);
            mockFileWrapper.Verify(x => x.Move("/foo/bar.txt", "/foo/b/bar.txt"), Times.Once);
        }
    }
}