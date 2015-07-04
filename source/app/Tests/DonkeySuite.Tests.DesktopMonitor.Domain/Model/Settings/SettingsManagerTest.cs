using System;
using System.IO;
using System.Threading;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using log4net;
using MadDonkeySoftware.SystemWrappers.Threading;
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
            public Mock<ISettingsRepository> MockSettingsRepository { get; private set; }
            public SettingsManager SettingsManager { get; private set; }

            public SettingsManagerTestBundle()
            {
                MockLog = new Mock<ILog>();
                MockSemaphore = new Mock<ISemaphore>();
                MockSettingsRepository = new Mock<ISettingsRepository>();

                SettingsManager = new SettingsManager(MockLog.Object, MockSemaphore.Object, MockSettingsRepository.Object);
            }
        }

        [Test]
        public void GetSettings_Returns_Settings_Object()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();
            var mockSettingsRoot = new Mock<SettingsRoot>();

            testBundle.MockSettingsRepository.Setup(x => x.Load()).Returns(mockSettingsRoot.Object);

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.IsNotNull(settings);
            Assert.AreSame(mockSettingsRoot.Object, settings);
        }

        [Test]
        public void GetSettings_Returns_New_Settings_Object_After_Save_Fails()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();
            var mockSettingsRoot = new Mock<SettingsRoot>();

            testBundle.MockSettingsRepository.Setup(x => x.Load()).Returns((SettingsRoot)null);
            testBundle.MockSettingsRepository.Setup(x => x.CreateNewSettings()).Returns(mockSettingsRoot.Object);
            testBundle.MockSettingsRepository.Setup(x => x.Save(mockSettingsRoot.Object)).Throws(new IOException("Test Exception"));

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, settings);
            testBundle.MockSemaphore.Verify(x => x.WaitOne(), Times.Once);
            testBundle.MockSemaphore.Verify(x => x.Release(), Times.Once);
        }

        [Test]
        public void GetSettings_Returns_New_Settings_Object_After_Save_Succeeds()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();
            var mockSettingsRoot = new Mock<SettingsRoot>();

            testBundle.MockSettingsRepository.Setup(x => x.Load()).Returns((SettingsRoot)null);
            testBundle.MockSettingsRepository.Setup(x => x.CreateNewSettings()).Returns(mockSettingsRoot.Object);

            // Act
            var settings = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, settings);
            testBundle.MockSemaphore.Verify(x => x.WaitOne(), Times.Once);
            testBundle.MockSemaphore.Verify(x => x.Release(), Times.Once);
        }

        [Test]
        public void GetSettings_Returns_Null_When_Semaphore_Interrupted()
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
        public void GetSettings_Rethrows_Exception()
        {
            var testBundle = new SettingsManagerTestBundle();
            var testException = new NullReferenceException();
            Exception caughtException = null;

            testBundle.MockSettingsRepository.Setup(x => x.Load()).Throws(testException);

            // Act
            try
            {
                testBundle.SettingsManager.GetSettings();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // Assert
            Assert.AreSame(testException, caughtException);
        }

        [Test]
        public void SettingsManagerReturnsSameSettingsObjectAfterMultipleCalls()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();
            var mockSettingsRoot = new Mock<SettingsRoot>();

            testBundle.MockSettingsRepository.Setup(x => x.Load()).Returns(mockSettingsRoot.Object);

            // Act
            var s1 = testBundle.SettingsManager.GetSettings();
            var s2 = testBundle.SettingsManager.GetSettings();

            // Assert
            Assert.AreSame(mockSettingsRoot.Object, s1);
            Assert.AreSame(mockSettingsRoot.Object, s2);
            Assert.AreSame(s1, s2);
        }

        [Test]
        public void SaveSettings_Locks_And_Unlocks_When_Save_Successful()
        {
            // Arrange
            var testBundle = new SettingsManagerTestBundle();

            // Act
            testBundle.SettingsManager.SaveSettings();

            // Assert
            testBundle.MockSemaphore.Verify(x => x.WaitOne(), Times.Once);
            testBundle.MockSemaphore.Verify(x => x.Release(), Times.Once);
        }

        [Test]
        public void SaveSettings_Throws_Exception_When_Semaphore_Fails()
        {
            // Arrange
            var testException = new ThreadInterruptedException();
            var testBundle = new SettingsManagerTestBundle();
            Exception thrownException = null;

            testBundle.MockSemaphore.Setup(x => x.WaitOne()).Throws(testException);

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
            testBundle.MockSemaphore.Verify(x => x.WaitOne(), Times.Once);
            testBundle.MockSemaphore.Verify(x => x.Release(), Times.Once);
            Assert.IsNotNull(thrownException);
        }

        [Test]
        public void SaveSettings_Throws_Exception_When_Serialization_Fails()
        {
            // Arrange
            var testException = new ThreadInterruptedException();
            var testBundle = new SettingsManagerTestBundle();
            Exception thrownException = null;

            testBundle.MockSettingsRepository.Setup(x => x.Save(It.IsAny<SettingsRoot>())).Throws(testException);

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
            testBundle.MockSemaphore.Verify(x => x.WaitOne(), Times.Once);
            testBundle.MockSemaphore.Verify(x => x.Release(), Times.Once);
            Assert.IsNotNull(thrownException);
        }
    }
}