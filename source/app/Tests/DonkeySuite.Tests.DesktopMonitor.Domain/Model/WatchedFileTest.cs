using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;
using Moq;
using Ninject;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model
{
    [TestFixture]
    public class WatchedFileTests
    {
        private class WatchedFileTestBundle
        {
            public Mock<ILog> MockLog { get; private set; }
            public Mock<IFile> MockFile { get; private set; }
            public Mock<IEntityProvider> MockServiceProvider { get; private set; }
            public Mock<IRequestProvider> MockRequestProvider { get; private set; }
            public Mock<IEnvironmentUtility> MockEnvironmentUtility { get; private set; }
            public Mock<IDirectory> MockDirectory { get; private set; }
            public WatchedFile WatchedFile { get; private set; }

            public WatchedFileTestBundle()
            {
                MockLog = new Mock<ILog>();
                MockFile = new Mock<IFile>();
                MockServiceProvider = new Mock<IEntityProvider>();
                MockRequestProvider = new Mock<IRequestProvider>();
                MockEnvironmentUtility = new Mock<IEnvironmentUtility>();
                MockDirectory = new Mock<IDirectory>();
                WatchedFile = new WatchedFile(MockLog.Object, MockServiceProvider.Object, MockFile.Object, MockRequestProvider.Object, MockEnvironmentUtility.Object, MockDirectory.Object);
            }
        }

        [Test]
        public void WatchedFileReturnsSetSortStrategyWhenGetSortStrategyCalled()
        {
            // Arrange
            var testBundle = new WatchedFileTestBundle();
            var strategy = new SimpleSortStrategy();

            // Act
            testBundle.WatchedFile.SortStrategy = strategy;

            // Assert
            Assert.AreSame(strategy, testBundle.WatchedFile.SortStrategy);
        }

        [Test]
        public void WatchedFileReturnsDefaultSortStrategyWhenGetSortStrategyCalled()
        {
            // Arrange
            var testBundle = new WatchedFileTestBundle();
            var strategy = new SimpleSortStrategy();

            testBundle.MockServiceProvider.Setup(x => x.ProvideSortStrategy(null)).Returns(strategy);

            // Act & Assert
            Assert.AreSame(strategy, testBundle.WatchedFile.SortStrategy);
        }

        [Test]
        public void WatchedFileLoadImageBytesReturnsFileDataWithDebugLogging()
        {
            // Arrange
            const string filePath = "/foo/bar/blah.txt";
            var testBundle = new WatchedFileTestBundle();

            testBundle.MockFile.Setup(x => x.ReadAllBytes(filePath)).Returns(new byte[] {1, 4, 3});
            testBundle.WatchedFile.FileName = "blah.txt";
            testBundle.WatchedFile.FullPath = filePath;

            // Act
            var fileBytes = testBundle.WatchedFile.LoadImageBytes();

            // Assert
            Assert.IsNotNull(fileBytes);
            Assert.AreEqual(3, fileBytes.Length);
            testBundle.MockLog.Verify(x => x.DebugFormat("Beginning LoadImage: \"{0}\"", "/foo/bar/blah.txt"), Times.Once);
            testBundle.MockLog.Verify(x => x.DebugFormat("Returning LoadImage: \"{0}\"", "/foo/bar/blah.txt"), Times.Once);
        }

        [Test]
        public void WatchedFileSendToServerTransmitsFileAndSetsUploadSuccessfulProperly()
        {
            // Arrange
            const string filePath = "/foo/bar/testFile.txt";
            byte[] fileBytes = {1, 4, 3};
            var mockAddImageRequest = new Mock<IAddImageRequest>();
            var testBundle = new WatchedFileTestBundle();
            var mockServer = new Mock<ImageServer>();

            testBundle.MockRequestProvider.Setup(x => x.ProvideNewAddImageRequest(mockServer.Object, "testFile.txt", fileBytes)).Returns(mockAddImageRequest.Object);
            mockAddImageRequest.Setup(x => x.Post()).Returns(true);
            testBundle.MockFile.Setup(x => x.ReadAllBytes(filePath)).Returns(fileBytes);
            testBundle.WatchedFile.FileName = "testFile.txt";
            testBundle.WatchedFile.FullPath = filePath;

            // Act
            testBundle.WatchedFile.SendToServer(mockServer.Object);

            // Assert
            Assert.AreEqual(true, testBundle.WatchedFile.UploadSuccessful);
            testBundle.MockLog.Verify(x => x.DebugFormat("Transmitting image: {0}", "/foo/bar/testFile.txt"), Times.Once);
        }

        [Test]
        public void WatchedFileIsInBaseDirectoryReturnsTrueWhenFileIsInBaseDirectory()
        {
            // Arrange
            const string fullPath = "C:\\foo.txt";
            const string baseDirectory = "C:\\";
            const string fileName = "foo.txt";
            var testBundle = new WatchedFileTestBundle();

            testBundle.WatchedFile.FileName = fileName;
            testBundle.WatchedFile.FullPath = fullPath;
            testBundle.MockEnvironmentUtility.Setup(x => x.CombinePath(baseDirectory, fileName)).Returns("C:\\foo.txt");

            // Act
            var rslt = testBundle.WatchedFile.IsInBaseDirectory(baseDirectory);

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
            var testBundle = new WatchedFileTestBundle();

            testBundle.WatchedFile.FileName = fileName;
            testBundle.WatchedFile.FullPath = fullPath;
            testBundle.MockEnvironmentUtility.Setup(x => x.CombinePath(baseDirectory, fileName)).Returns("C:\\foo.txt");

            // Act
            var rslt = testBundle.WatchedFile.IsInBaseDirectory(baseDirectory);

            // Assert
            Assert.AreEqual(false, rslt);
        }

        [Test]
        public void WatchedFileSortFileThrowsExceptionWhenNoDefaultSortStrategyDefined()
        {
            // Arrange
            var testBundle = new WatchedFileTestBundle();

            testBundle.MockEnvironmentUtility.SetupGet(x => x.DirectorySeparatorChar).Returns('/');
            testBundle.MockServiceProvider.Setup(x => x.ProvideSortStrategy(null)).Throws(new ActivationException());

            // Act
            testBundle.WatchedFile.FullPath = "/foo/bar.txt";
            testBundle.WatchedFile.FileName = "bar.txt";
            testBundle.WatchedFile.UploadSuccessful = true;

            try
            {
                testBundle.WatchedFile.SortFile();
            }
            catch (ActivationException ex)
            {
                Assert.AreEqual("Exception of type 'Ninject.ActivationException' was thrown.", ex.Message);
            }
        }

        [Test]
        public void WatchedFileSortFileLogsMessageWhenDestinationAlreadyHasFileWithSameName()
        {
            // Arrange
            var mockSortStrategy = new Mock<ISortStrategy>();
            var testBundle = new WatchedFileTestBundle();

            testBundle.MockEnvironmentUtility.SetupGet(x => x.DirectorySeparatorChar).Returns('/');
            testBundle.MockFile.Setup(x => x.Exists("/foo/b/bar.txt")).Returns(true);

            mockSortStrategy.Setup(x => x.NewFileName("/foo", "bar.txt")).Returns("/foo/b/bar.txt");

            testBundle.WatchedFile.FullPath = "/foo/bar.txt";
            testBundle.WatchedFile.FileName = "bar.txt";
            testBundle.WatchedFile.SortStrategy = mockSortStrategy.Object;
            testBundle.WatchedFile.UploadSuccessful = true;

            // Act
            testBundle.WatchedFile.SortFile();

            // Assert
            testBundle.MockLog.Verify(x => x.DebugFormat("Moving file failed due to existing file in destination. File name: {0}", "bar.txt"), Times.Once);
        }

        [Test]
        public void WatchedFileSortFileMovesFileBasedOnSortStrategyWhenDestinationIsClear()
        {
            // Arrange
            var mockSortStrategy = new Mock<ISortStrategy>();
            var testBundle = new WatchedFileTestBundle();

            testBundle.MockEnvironmentUtility.SetupGet(x => x.DirectorySeparatorChar).Returns('/');
            testBundle.MockFile.Setup(x => x.Exists("/foo/b/bar.txt")).Returns(false);

            mockSortStrategy.Setup(x => x.NewFileName("/foo", "bar.txt")).Returns("/foo/b/bar.txt");

            testBundle.WatchedFile.FullPath = "/foo/bar.txt";
            testBundle.WatchedFile.FileName = "bar.txt";
            testBundle.WatchedFile.SortStrategy = mockSortStrategy.Object;
            testBundle.WatchedFile.UploadSuccessful = true;

            // Act
            testBundle.WatchedFile.SortFile();

            // Assert
            testBundle.MockLog.Verify(x => x.DebugFormat("Renaming file. From: {0} To: {1}", "/foo/bar.txt", "/foo/b/bar.txt"), Times.Once);
            testBundle.MockDirectory.Verify(x => x.CreateDirectory("/foo/b"), Times.Once);
            testBundle.MockFile.Verify(x => x.Move("/foo/bar.txt", "/foo/b/bar.txt"), Times.Once);
        }

        [Test]
        public void WatchedFileParameterlessConstructorCreatesObject()
        {
            // I know this test may seem silly but NHibernate requires a parameterless constructor on objects it persists.
            var obj = new WatchedFile();
            Assert.IsNotNull(obj);
        }

        [Test]
        public void WatchedFileRemoveFromDiskCalls()
        {
            // Arrange
            var testBundle = new WatchedFileTestBundle();
            testBundle.WatchedFile.FullPath = "/foo.txt";

            // Act
            testBundle.WatchedFile.RemoveFromDisk();

            // Assert
            testBundle.MockFile.Verify(x => x.Delete("/foo.txt"), Times.Once);
        }
    }
}