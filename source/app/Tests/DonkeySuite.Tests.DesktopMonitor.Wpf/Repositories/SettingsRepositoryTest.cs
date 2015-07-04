using System;
using System.IO;
using System.Runtime.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Wpf.Repositories;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;
using MadDonkeySoftware.SystemWrappers.Xml.Serialization;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Wpf.Repositories
{
    [TestFixture]
    public class SettingsRepositoryTest
    {
        private class SettingsRepositoryTestBundle
        {
            private SettingsRepository _settingsRepository;

            public Mock<ILogProvider> MockLogProvider { get; private set; }
            public Mock<IEntityProvider> MockEntityProvider { get; private set; }
            public Mock<IXmlSerializer> MockXmlSerializer { get; private set; }
            public Mock<IFile> MockFile { get; private set; }
            public Mock<IEnvironmentUtility> MockEnvironmentUtility { get; private set; }

            public SettingsRepository SettingsManager
            {
                get
                {
                    return _settingsRepository ??
                           (_settingsRepository = new SettingsRepository(MockLogProvider.Object, MockEntityProvider.Object, MockXmlSerializer.Object, MockFile.Object, MockEnvironmentUtility.Object));
                }
            }

            public SettingsRepositoryTestBundle()
            {
                MockLogProvider = new Mock<ILogProvider>();
                MockEntityProvider = new Mock<IEntityProvider>();
                MockXmlSerializer = new Mock<IXmlSerializer>();
                MockFile = new Mock<IFile>();
                MockEnvironmentUtility = new Mock<IEnvironmentUtility>();
            }
        }

        [Test]
        public void Load_Returns_Null_When_File_Does_Not_Exist()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var mockLog = new Mock<ILog>();

            testBundle.MockFile.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);

            testBundle.MockXmlSerializer.Setup(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()))
                .Throws(new SerializationException());

            // Act
            var settings = testBundle.SettingsManager.Load();

            // Assert
            Assert.IsNull(settings);
        }

        [Test]
        public void Load_Rethrows_Exception()
        {
            // Arrange
            var mockLogProvider = new Mock<ILogProvider>();
            NullReferenceException exception = null;

            // Act
            try
            {
                var settingManager = new SettingsRepository(mockLogProvider.Object, null, null, null, null);
                settingManager.Load();
            }
            catch (NullReferenceException ex)
            {
                exception = ex;
            }
            // Assert
            Assert.IsNotNull(exception);
        }

        [Test]
        public void Load_Returns_Same_Settings_Object_After_Multiple_Calls()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var mockSettingsRoot = new Mock<SettingsRoot>();
            var mockLog = new Mock<ILog>();

            testBundle.MockXmlSerializer.Setup(x => x.Deserialize(It.IsAny<Stream>())).Returns(mockSettingsRoot.Object);
            testBundle.MockFile.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);

            // Act
            var s1 = testBundle.SettingsManager.Load();
            var s2 = testBundle.SettingsManager.Load();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, s1);
            Assert.AreSame(mockSettingsRoot.Object, s2);
            Assert.AreSame(s1, s2);
        }

        [Test]
        public void Load_Returns_Saved_Settings_Object()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var mockSettingsRoot = new Mock<SettingsRoot>();
            var mockLog = new Mock<ILog>();

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);
            testBundle.MockFile.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            testBundle.MockEnvironmentUtility.SetupGet(x => x.IsWindowsPlatform).Returns(true);
            testBundle.MockXmlSerializer.Setup(x => x.Deserialize(It.IsAny<Stream>())).Returns(mockSettingsRoot.Object);

            // Act
            var settings = testBundle.SettingsManager.Load();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, settings);
            testBundle.MockXmlSerializer.Verify(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()), Times.Never);
        }

        [Test]
        public void Load_Returns_Null_When_Failure_To_Read_Settings_File()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var mockLog = new Mock<ILog>();

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);
            testBundle.MockFile.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);
            testBundle.MockEnvironmentUtility.SetupGet(x => x.IsWindowsPlatform).Returns(true);
            testBundle.MockXmlSerializer.Setup(x => x.Deserialize(It.IsAny<Stream>())).Throws(new SerializationException());

            // Act
            var settings = testBundle.SettingsManager.Load();

            // Assert
            Assert.IsNull(settings);
            mockLog.Verify(x => x.Error("Error opening settings file.", It.IsAny<Exception>()), Times.Once);
        }

        [Test]
        public void Save_Writes_To_Serializer()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var mockLog = new Mock<ILog>();
            var settings = new SettingsRoot();

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);

            // Act
            testBundle.SettingsManager.Save(settings);

            // Assert
            testBundle.MockXmlSerializer.Verify(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void Save_Rethrows_Exception()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var mockLog = new Mock<ILog>();
            var settings = new SettingsRoot();
            var testException = new Exception("test exception");
            Exception thrownException = null;

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);
            testBundle.MockXmlSerializer.Setup(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>())).Throws(testException);

            // Act
            try
            {
                testBundle.SettingsManager.Save(settings);
            }
            catch (Exception ex)
            {
                thrownException = ex;
            }

            // Assert
            testBundle.MockXmlSerializer.Verify(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()), Times.Once);
            Assert.AreEqual(testException, thrownException);
        }

        [Test]
        public void Save_Wraps_SerializationException_As_IOException()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var mockLog = new Mock<ILog>();
            var settings = new SettingsRoot();
            var testException = new SerializationException("test exception");
            IOException thrownException = null;

            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);
            testBundle.MockXmlSerializer.Setup(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>())).Throws(testException);

            // Act
            try
            {
                testBundle.SettingsManager.Save(settings);
            }
            catch (IOException ex)
            {
                thrownException = ex;
            }

            // Assert
            testBundle.MockXmlSerializer.Verify(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()), Times.Once);
            Assert.IsNotNull(thrownException);
        }

        [Test]
        public void CreateNewSettings_Returns_New_Object()
        {
            // Arrange
            var testBundle = new SettingsRepositoryTestBundle();
            var settingsRoot = new SettingsRoot();

            testBundle.MockEntityProvider.Setup(x => x.ProvideDefaultSettingsRoot()).Returns(settingsRoot);

            // Act
            var settings = testBundle.SettingsManager.CreateNewSettings();

            // Assert
            Assert.AreSame(settingsRoot, settings);
            testBundle.MockEntityProvider.Verify(x => x.ProvideDefaultSettingsRoot(), Times.Once);
        }
    }
}