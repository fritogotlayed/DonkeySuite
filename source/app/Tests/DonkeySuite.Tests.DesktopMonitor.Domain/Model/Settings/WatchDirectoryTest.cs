using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using MadDonkeySoftware.SystemWrappers;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class WatchDirectoryTest
    {
        private class WatchDirectoryTestBundle
        {
            public Mock<IEnvironment> MockEnvironment { get; private set; }
            public WatchDirectory WatchDirectory { get; private set; }

            public WatchDirectoryTestBundle()
            {
                MockEnvironment = new Mock<IEnvironment>();
                WatchDirectory = new WatchDirectory(MockEnvironment.Object);
            }
        }

        [Test]
        public void WatchDirectory_ConstructedWithDefaults()
        {
            // Arrange & Act
            var testBundle = new WatchDirectoryTestBundle();

            // Assert
            Assert.AreEqual(null, testBundle.WatchDirectory.FileExtensions, "FileExtensions");
            Assert.AreEqual(null, testBundle.WatchDirectory.Path, "Path");
            Assert.AreEqual(null, testBundle.WatchDirectory.SortStrategy, "SortStrategy");
            Assert.AreEqual(OperationMode.Unknown, testBundle.WatchDirectory.Mode, "Mode");
            Assert.AreEqual(false, testBundle.WatchDirectory.IncludeSubDirectories, "IncludeSubDirectories");
        }

        [Test]
        public void WatchDirectory_PopulateWithDefaults_OnWindows_ConfiguresObjectProperly()
        {
            // Arrange
            var testBundle = new WatchDirectoryTestBundle();

            testBundle.MockEnvironment.SetupGet(x => x.IsWindowsPlatform).Returns(true);

            // Act
            testBundle.WatchDirectory.PopulateWithDefaults();

            // Assert
            const string expectedDirectory = "C:\\";
            Assert.AreEqual("jpg,jpeg,gif,tiff", testBundle.WatchDirectory.FileExtensions, "FileExtensions");
            Assert.AreEqual(expectedDirectory, testBundle.WatchDirectory.Path, "Path");
            Assert.AreEqual("Simple", testBundle.WatchDirectory.SortStrategy, "SortStrategy");
            Assert.AreEqual(OperationMode.Unknown, testBundle.WatchDirectory.Mode, "Mode");
            Assert.AreEqual(false, testBundle.WatchDirectory.IncludeSubDirectories, "IncludeSubDirectories");
        }

        [Test]
        public void WatchDirectory_PopulateWithDefaults_NotOnWindows_ConfiguresObjectProperly()
        {
            // Arrange
            var testBundle = new WatchDirectoryTestBundle();

            testBundle.MockEnvironment.SetupGet(x => x.IsWindowsPlatform).Returns(false);

            // Act
            testBundle.WatchDirectory.PopulateWithDefaults();

            // Assert
            const string expectedDirectory = "/";
            Assert.AreEqual("jpg,jpeg,gif,tiff", testBundle.WatchDirectory.FileExtensions, "FileExtensions");
            Assert.AreEqual(expectedDirectory, testBundle.WatchDirectory.Path, "Path");
            Assert.AreEqual("Simple", testBundle.WatchDirectory.SortStrategy, "SortStrategy");
            Assert.AreEqual(OperationMode.Unknown, testBundle.WatchDirectory.Mode, "Mode");
            Assert.AreEqual(false, testBundle.WatchDirectory.IncludeSubDirectories, "IncludeSubDirectories");
        }

        [Test]
        public void WatchDirectory_ObjectSettersWorkCorrectly()
        {
            // Arrange
            var testBunle = new WatchDirectoryTestBundle();

            // Act
            testBunle.WatchDirectory.FileExtensions = "jpg,jpeg";
            testBunle.WatchDirectory.IncludeSubDirectories = true;
            testBunle.WatchDirectory.Mode = OperationMode.SortOnly;
            testBunle.WatchDirectory.Path = "D:\\";
            testBunle.WatchDirectory.SortStrategy = "SortStrategy";

            // Assert
            Assert.AreEqual("jpg,jpeg", testBunle.WatchDirectory.FileExtensions, "FileExtensions");
            Assert.AreEqual("D:\\", testBunle.WatchDirectory.Path, "Path");
            Assert.AreEqual("SortStrategy", testBunle.WatchDirectory.SortStrategy, "SortStrategy");
            Assert.AreEqual(OperationMode.SortOnly, testBunle.WatchDirectory.Mode, "Mode");
            Assert.AreEqual(true, testBunle.WatchDirectory.IncludeSubDirectories, "IncludeSubDirectories");
        }
    }
}