using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.Wrappers;
using log4net;
using Moq;
using Ninject;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class SettingsManagerTest : TestBase
    {
        [Test]
        public void SettingsManagerInstanceIsNotNull()
        {
            DependencyManager.Kernel = null;
            Assert.IsNotNull(DependencyManager.Kernel.Get<SettingsManager>());
        }

        [Test]
        public void SettingsManagerMultipleCallsToInstanceReturnsTheSameObject()
        {
            // Arrange
            DependencyManager.Kernel = null;
            // Act
            var m1 = DependencyManager.Kernel.Get<SettingsManager>();
            var m2 = DependencyManager.Kernel.Get<SettingsManager>();

            // Assert
            Assert.AreSame(m1, m2);
        }

        [Test]
        public void SettingsManagerReturnsSettingsObject()
        {
            // Arrange
            var mockSerializer = new Mock<ISerializer>();
            var mockFileWrapper = new Mock<IFileWrapper>();
            var mockStreamWriter = new Mock<TextWriter>();
            var mockLogger = new Mock<ILog>();
            var mockSemaphore = new Mock<ISemaphoreWrapper>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();

            mockFileWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            mockFileWrapper.Setup(x => x.Open(It.IsAny<string>(), FileMode.Open)).Returns((FileStream)null);
            mockEnvironmentWrapper.SetupGet(x => x.IsWindowsPlatform).Returns(true);

            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);
            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<TextWriter>().ToMethod(context => mockStreamWriter.Object);
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settings = DependencyManager.Kernel.Get<SettingsManager>().GetSettings();

            // Assert
            Assert.IsNotNull(settings);
        }

        [Test]
        public void SettingsManagerReturnsNewSettingsObjectAfterSaveFails()
        {
            // Arrange
            var mockSerializer = new Mock<ISerializer>();
            var mockFileWrapper = new Mock<IFileWrapper>();
            var mockStreamWriter = new Mock<TextWriter>();
            var mockLogger = new Mock<ILog>();
            var mockSemaphore = new Mock<ISemaphoreWrapper>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();
            var settingsRoot = new SettingsRoot();

            mockFileWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            mockFileWrapper.Setup(x => x.Open(It.IsAny<string>(), FileMode.Open)).Returns((FileStream)null);
            mockEnvironmentWrapper.SetupGet(x => x.IsWindowsPlatform).Returns(true);

            mockSerializer.Setup(x => x.Serialize(It.IsAny<TextWriter>(), It.IsAny<object>()))
                .Throws(new SerializationException());

            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);
            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<TextWriter>().ToMethod(context => mockStreamWriter.Object);
            TestKernel.Bind<SettingsRoot>().ToMethod(context => settingsRoot);
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            var settings = settingManager.GetSettings();

            // Assert
            Assert.AreSame(settings, settingsRoot);
        }

        [Test]
        public void SettingsManagerReturnsNullWhenSemaphoreInterrupted()
        {
            // Arrange
            var mockSemaphore = new Mock<ISemaphoreWrapper>();
            var mockLogger = new Mock<ILog>();

            mockSemaphore.Setup(x => x.WaitOne()).Throws(new AbandonedMutexException());

            TestKernel.Bind<ISemaphoreWrapper>().ToMethod(context => mockSemaphore.Object);
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            var settings = settingManager.GetSettings();

            // Assert
            Assert.IsNull(settings);
        }

        [Test]
        public void SettingsManagerRethrowsException()
        {
            // Act
            try
            {
                var settingManager = new SettingsManager(null, null);
                settingManager.GetSettings();
            }
            catch (Exception ex)
            {
                // Assert
                Assert.AreEqual(typeof (NullReferenceException), ex.GetType());
            }
        }

        [Test]
        public void SettingsManagerReturnsSameSettingsObjectAfterMultipleCalls() {
            // Arrange
            var mockSerializer = new Mock<ISerializer>();
            var mockFileWrapper = new Mock<IFileWrapper>();
            var mockStreamWriter = new Mock<TextWriter>();
            var mockLogger = new Mock<ILog>();
            var mockSemaphore = new Mock<ISemaphoreWrapper>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();
            var settingsRoot = new SettingsRoot();

            mockFileWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(false);
            mockEnvironmentWrapper.SetupGet(x => x.IsWindowsPlatform).Returns(true);

            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);
            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<TextWriter>().ToMethod(context => mockStreamWriter.Object);
            TestKernel.Bind<SettingsRoot>().ToMethod(context => settingsRoot);
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            var s1 = settingManager.GetSettings();
            var s2 = settingManager.GetSettings();

            // Assert
            Assert.AreSame(s1, s2);
        }

        [Test]
        public void SettingsManagerReturnsSavedSettingsObject()
        {
            // Arrange
            var mockSerializer = new Mock<ISerializer>();
            var mockFileWrapper = new Mock<IFileWrapper>();
            var mockStreamWriter = new Mock<TextWriter>();
            var mockLogger = new Mock<ILog>();
            var mockSemaphore = new Mock<ISemaphoreWrapper>();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();
            var settingsRoot = new SettingsRoot();

            mockEnvironmentWrapper.SetupGet(x => x.IsWindowsPlatform).Returns(true);

            TestKernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);
            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<TextWriter>().ToMethod(context => mockStreamWriter.Object);
            TestKernel.Bind<SettingsRoot>().ToMethod(context => settingsRoot);
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            var settings = settingManager.GetSettings();

            // Assert
            Assert.AreSame(settingsRoot, settings);
        }

        [Test]
        public void SettingsManagerReturnsNullWhenFailureToReadSettingsFile()
        {
            // Arrange
            var mockSerializer = new Mock<ISerializer>();
            var mockFileWrapper = new Mock<IFileWrapper>();
            var mockLogger = new Mock<ILog>();
            var mockSemaphore = new Mock<ISemaphoreWrapper>();

            mockFileWrapper.Setup(x => x.Exists(It.IsAny<string>())).Returns(true);

            TestKernel.Bind<IFileWrapper>().ToMethod(context => mockFileWrapper.Object);
            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            var settings = settingManager.GetSettings();

            // Assert
            Assert.IsNull(settings);
        }

        [Test]
        public void SettingsManagerSaveWritesToSerializer()
        {
            // Arrange
            var mockSerializer = new Mock<ISerializer>();
            var mockStreamWriter = new Mock<TextWriter>();
            var mockLogger = new Mock<ILog>();
            var mockSemaphore = new Mock<ISemaphoreWrapper>();

            mockSerializer.Setup(x => x.Serialize(mockStreamWriter.Object, It.IsAny<object>()));

            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<TextWriter>().ToMethod(context => mockStreamWriter.Object);
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            settingManager.SaveSettings();
        }

        [Test] [ExpectedException(typeof(Exception))]
        public void SettingsManagerSaveRethrowsException()
        {
            // Arrange
            var mockSerializer = new Mock<ISerializer>();
            var mockStreamWriter = new Mock<TextWriter>();
            var mockLogger = new Mock<ILog>();
            var mockSemaphore = new Mock<ISemaphoreWrapper>();

            mockSerializer.Setup(x => x.Serialize(mockStreamWriter.Object, It.IsAny<object>())).Throws(new Exception("test exception"));

            TestKernel.Bind<ISerializer>().ToMethod(context => mockSerializer.Object).Named("SettingsSerializer");
            TestKernel.Bind<TextWriter>().ToMethod(context => mockStreamWriter.Object);
            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            settingManager.SaveSettings();
        }

        [Test]
        public void SettingsManagerSaveHandlesSemaphoreInterruptedGracefully()
        {
            // Arrange
            var mockSemaphore = new Mock<ISemaphoreWrapper>();
            var mockLogger = new Mock<ILog>();

            mockSemaphore.Setup(x => x.WaitOne()).Throws(new ThreadInterruptedException());

            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            settingManager.SaveSettings();
        }

        [Test] [ExpectedException(typeof(IOException))]
        public void SettingsManagerSaveThrowsAbortedExceptionWhenSerializationFails()
        {
            // Arrange
            var mockSemaphore = new Mock<ISemaphoreWrapper>();
            var mockLogger = new Mock<ILog>();

            mockSemaphore.Setup(x => x.WaitOne()).Throws(new SerializationException());

            TestKernel.Bind<SettingsManager>().ToSelf().InTransientScope()
                .WithConstructorArgument("log", mockLogger.Object)
                .WithConstructorArgument("semaphore", mockSemaphore.Object);

            // Act
            var settingManager = DependencyManager.Kernel.Get<SettingsManager>();
            settingManager.SaveSettings();
        }
    }
}