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

            // Act & Assert
            var watchedFile = new WatchedFile {SortStrategy = strategy};
            Assert.AreSame(strategy, watchedFile.SortStrategy);
        }

        [Test]
        public void WatchedFileReturnsDefaultSortStrategyWhenGetSortStrategyCalled()
        {
            // Arrange
            var strategy = new SimpleSortStrategy();
            var watchedFile = new WatchedFile();

            TestKernel.Bind<ISortStrategy>().ToMethod(context => strategy).Named("defaultSortStrategy");

            // Act & Assert
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

            var f = new WatchedFile();
            f.FileName = fileName;
            f.FullPath = fullPath;

            // Act
            var rslt = f.IsInBaseDirectory(baseDirectory);

            // Assert
            Assert.AreEqual(false, rslt);
        }
    }
}
/*
public class WatchedFileTest {
    @Test
    public final void watchedFileSortFileThrowsExceptionWhenNoDefaultSortStrategyDefined(){
        // Arrange
        WatchedFile f = new WatchedFile();

        try{
                // Act
                f.sortFile();
        }catch(Exception ex){
                // Assert
            assertTrue(ex instanceof IllegalStateException);
            assertEquals("Sort strategy is not set.", ex.getMessage());
        }
    }

    @Test
    public final void watchedFileSortFileLogsMessageWhenDestinationAlreadyHasFileWithSameName(){
        // Arrange
        Log logMock = mock(Log.class);
        File oldFileMock = mock(File.class);
        File newFileMock = mock(File.class);
        SortStrategy strategy = new SimpleSortStrategy();
        String fileName = "file.txt";
        String oldFilePath;
        final List<String> logMessages = new ArrayList<String>();

        if(File.separator.equals("\\")){
            oldFilePath = "c:\\some\\file\\path\\" + fileName;
        }else{
            oldFilePath = "/some/file/path/" + fileName;
        }

        when(oldFileMock.getPath()).thenReturn(oldFilePath);
        when(newFileMock.exists()).thenReturn(true);
        MockBeanProvider.enqueueMockLogger(logMock);
        MockBeanProvider.enqueueMockLogger(logMock);
        MockBeanProvider.enqueueOneArgFile(oldFileMock);
        MockBeanProvider.enqueueOneArgFile(newFileMock);

        doAnswer(new Answer(){
            public Object answer(InvocationOnMock invocation) throws Throwable {
                Object[] args = invocation.getArguments();
                logMessages.add((String)args[0]);
                return null;
            }
        }).when(logMock).trace(any(String.class));
        WatchedFile f = new WatchedFile();
        f.setSortStrategy(strategy);
        f.setFileName(fileName);

        // Act
        f.sortFile();

        // Assert
        assertEquals("Moving file failed due to existing file in destination. File name: " + fileName, logMessages.get(0));
    }

    @Test
    public final void watchedFileSortFileMovesFileBasedOnSortStrategyWhenDestinationIsClear(){
        // Arrange
        Log logMock = mock(Log.class);
        File oldFileMock = mock(File.class);
        File newFileMock = mock(File.class);
        File tempFileMock = mock(File.class);
        SortStrategy strategy = new SimpleSortStrategy();
        String fileName = "file.txt";
        String oldFilePath;
        String newFilePath;
        String newFileParentPath;
        final List<String> logMessages = new ArrayList<String>();

        if(System.getProperty("os.name").contains("Windows")){
            oldFilePath = "c:\\some\\file\\path\\" + fileName;
            newFileParentPath = "c:\\some\\file\\path\\p\\";
            newFilePath = newFileParentPath + fileName;
        }else{
            oldFilePath = "/some/file/path/" + fileName;
            newFileParentPath = "/some/file/path/p/";
            newFilePath = newFileParentPath + fileName;
        }

        when(oldFileMock.getPath()).thenReturn(oldFilePath);
        when(newFileMock.exists()).thenReturn(false);
        when(newFileMock.getPath()).thenReturn(newFilePath);
        when(newFileMock.getParent()).thenReturn(newFileParentPath);
        MockBeanProvider.enqueueMockLogger(logMock);
        MockBeanProvider.enqueueMockLogger(logMock);
        MockBeanProvider.enqueueOneArgFile(oldFileMock);
        MockBeanProvider.enqueueOneArgFile(newFileMock);
        MockBeanProvider.enqueueOneArgFile(tempFileMock);

        doAnswer(new Answer(){
            public Object answer(InvocationOnMock invocation) throws Throwable {
                Object[] args = invocation.getArguments();
                logMessages.add((String)args[0]);
                return null;
            }
        }).when(logMock).trace(any(String.class));
        WatchedFile f = new WatchedFile();
        f.setSortStrategy(strategy);
        f.setFileName(fileName);

        // Act
        f.sortFile();

        // Assert
        assertEquals("Renaming file. From: " + oldFilePath + " To: " + newFilePath, logMessages.get(0));
        verify(oldFileMock).renameTo(newFileMock);
    }

    private final File createTestFile() throws Exception{
        BufferedWriter writer = null;
        File tempFile = null;
        try {
            // Create a temporary file
            tempFile = new File(testFileName);

            // Write data to the file
            writer = new BufferedWriter(new FileWriter(tempFile));
            writer.write("test file data.");
        } catch (Exception e) {
            throw e;
        } finally {
            try {
                // Close the writer regardless of what happens...
                writer.close();
            } catch (Exception e) {
            }
        }

        return tempFile;
    }
}
*/