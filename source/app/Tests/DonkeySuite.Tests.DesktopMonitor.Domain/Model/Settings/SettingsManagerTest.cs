using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using log4net;
using MadDonkeySoftware.SystemWrappers;
using MadDonkeySoftware.SystemWrappers.IO;
using MadDonkeySoftware.SystemWrappers.Threading;
using MadDonkeySoftware.SystemWrappers.Xml.Serialization;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class SettingsManagerTest
    {
        private class SettingsManagerTestBundle
        {
            public Mock<ILog> MockLog { get; private set; }
            public Mock<ISemaphore> MockSemaphore { get; private set; }
            public Mock<IFile> MockFile { get; private set; }
            public Mock<IXmlSerializer> MockSerializer { get; private set; }
            public Mock<IEnvironmentUtility> MockEnvironmentUtility { get; private set; }
            public Mock<IServiceLocator> MockServiceLocator { get; private set; }
            public SettingsManager SettingsManager { get; private set; }

            public SettingsManagerTestBundle()
            {
                MockLog = new Mock<ILog>();
                MockSemaphore = new Mock<ISemaphore>();
                MockFile = new Mock<IFile>();
                MockSerializer = new Mock<IXmlSerializer>();
                MockEnvironmentUtility = new Mock<IEnvironmentUtility>();
                MockServiceLocator = new Mock<IServiceLocator>();

                SettingsManager = new SettingsManager(MockLog.Object, MockSemaphore.Object, MockFile.Object, MockSerializer.Object, MockEnvironmentUtility.Object, MockServiceLocator.Object);
            }
        }

        [Test]
        public void SettingsManagerReturnsSettingsObject()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();
            var mockSettingsRoot = new Mock<SettingsRoot>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultSettingsRoot()).Returns(mockSettingsRoot.Object);

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.IsNotNull(settings);
        }

        [Test]
        public void SettingsManagerReturnsNewSettingsObjectAfterSaveFails()
        {
            // Arrange
            var mockSettingsRoot = new Mock<SettingsRoot>();
            var testBundle = new SettingsManagerTestBundle();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultSettingsRoot()).Returns(mockSettingsRoot.Object);
            testBundle.MockFile.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            testBundle.MockFile.Setup(x => x.Open(It.IsAny<string>(), FileMode.Open)).Returns((FileStream)null);
            testBundle.MockEnvironmentUtility.SetupGet(x => x.IsWindowsPlatform).Returns(true);

            testBundle.MockSerializer.Setup(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()))
                .Throws(new SerializationException());

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, settings);
        }

        [Test]
        public void SettingsManagerReturnsNullWhenSemaphoreInterrupted()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();

            testBundle.MockSemaphore.Setup(x => x.WaitOne()).Throws(new AbandonedMutexException());

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.IsNull(settings);
        }

        [Test]
        public void SettingsManagerRethrowsException()
        {
            // Act
            try
            {
                var settingManager = new SettingsManager(null, null, null, null, null, null);
                settingManager.GetSettings();
            }
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual(typeof (NullReferenceException), ex.GetType());
            }
        }

        [Test]
        public void SettingsManagerReturnsSameSettingsObjectAfterMultipleCalls()
        {
            // Arrange
            var mockSettingsRoot = new Mock<SettingsRoot>();
            var testBundle = new SettingsManagerTestBundle();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultSettingsRoot()).Returns(mockSettingsRoot.Object);

            // Act
            var s1 = testBundle.SettingsManager.GetSettings();
            var s2 = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, s1);
            Assert.AreSame(mockSettingsRoot.Object, s2);
            Assert.AreSame(s1, s2);
        }

        [Test]
        public void SettingsManagerReturnsSavedSettingsObject()
        {
            // Arrange
            var mockSettingsRoot = new Mock<SettingsRoot>();
            var testBundle = new SettingsManagerTestBundle();

            testBundle.MockEnvironmentUtility.SetupGet(x => x.IsWindowsPlatform).Returns(true);
            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultSettingsRoot()).Returns(mockSettingsRoot.Object);

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, settings);
            testBundle.MockSerializer.Verify(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void SettingsManagerReturnsNullWhenFailureToReadSettingsFile()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();

            testBundle.MockFile.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            testBundle.MockEnvironmentUtility.SetupGet(x => x.IsWindowsPlatform).Returns(true);
            testBundle.MockSerializer.Setup(x => x.Deserialize(It.IsAny<Stream>())).Throws(new SerializationException());

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.IsNull(settings);
            testBundle.MockLog.Verify(x => x.Error("Error opening settings file.", It.IsAny<Exception>()), Times.Once);
        }

        [Test]
        public void SettingsManagerSaveWritesToSerializer()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();

            // Act
            testBundle.SettingsManager.SaveSettings();

            // Assert
            testBundle.MockSerializer.Verify(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void SettingsManagerSaveRethrowsException()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();
            var testException = new Exception("test exception");
            Exception thrownException = null;

            testBundle.MockSerializer.Setup(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>())).Throws(testException);

            // Act
            try
            {
                testBundle.SettingsManager.SaveSettings();
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }

            // Assert
            testBundle.MockSerializer.Verify(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()), Times.Once);
            Assert.AreEqual(testException, thrownException);
        }

        [Test]
        public void SettingsManagerSaveHandlesSemaphoreInterruptedGracefully()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();

            testBundle.MockSemaphore.Setup(x => x.WaitOne()).Throws(new ThreadInterruptedException());

            // Act
            testBundle.SettingsManager.SaveSettings();

            // Assert
            testBundle.MockSemaphore.Verify(x => x.WaitOne(), Times.Once);
        }

        [Test]
        public void SettingsManagerSaveThrowsIOExceptionWhenSerializationFails()
        {
            // Arrange
            var testException = new SerializationException();
            var testBundle = new SettingsManagerTestBundle();
            IOException thrownException = null;

            testBundle.MockSemaphore.Setup(x => x.WaitOne()).Throws(testException);

            // Act
            try
            {
                testBundle.SettingsManager.SaveSettings();
            }
            catch (IOException ex)
            {
                thrownException = ex;
            }

            // Assert
            testBundle.MockSemaphore.Verify(x => x.WaitOne(), Times.Once);
            Assert.IsNotNull(thrownException);
        }
    }
}