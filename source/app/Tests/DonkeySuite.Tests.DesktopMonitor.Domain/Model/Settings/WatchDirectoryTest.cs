using System;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.SystemWrappers.Interfaces;
using Moq;
using Ninject;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class WatchDirectoryTest
    {

        [SetUp]
        public void SetUp()
        {
            DependencyManager.Kernel = new StandardKernel();
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void WatchDirectory_ConstructedWithDefaults()
        {
            // Act
            var dir = new WatchDirectory();

            // Assert
            Assert.AreEqual(null, dir.FileExtensions, "FileExtensions");
            Assert.AreEqual(null, dir.Path, "Path");
            Assert.AreEqual(null, dir.SortStrategy, "SortStrategy");
            Assert.AreEqual(OperationMode.Unknown, dir.Mode, "Mode");
            Assert.AreEqual(false, dir.IncludeSubDirectories, "IncludeSubDirectories");
        }

        [Test]
        public void WatchDirectory_PopulateWithDefaults_ConfiguresObjectProperly()
        {
            // Arrange
            var dir = new WatchDirectory();
            var mockEnvironmentWrapper = new Mock<IEnvironment>();

            mockEnvironmentWrapper.SetupGet(x => x.IsWindowsPlatform).Returns(true);
            DependencyManager.Kernel.Bind<IEnvironment>().ToMethod(context => mockEnvironmentWrapper.Object);

            // Act
            dir.PopulateWithDefaults();

            // Assert
            var expectedDirectory = Utilities.IsWindowsPlatform ? "C:\\" : "/";
            Assert.AreEqual("jpg,jpeg,gif,tiff", dir.FileExtensions, "FileExtensions");
            Assert.AreEqual(expectedDirectory, dir.Path, "Path");
            Assert.AreEqual("Simple", dir.SortStrategy, "SortStrategy");
            Assert.AreEqual(OperationMode.Unknown, dir.Mode, "Mode");
            Assert.AreEqual(false, dir.IncludeSubDirectories, "IncludeSubDirectories");
        }

        [Test]
        public void WatchDirectory_ObjectSettersWorkCorrectly()
        {
            // Act
            var dir = new WatchDirectory
            {
                FileExtensions = "jpg,jpeg",
                IncludeSubDirectories = true,
                Mode = OperationMode.SortOnly,
                Path = "D:\\",
                SortStrategy = "SortStrategy"
            };

            // Assert
            Assert.AreEqual("jpg,jpeg", dir.FileExtensions, "FileExtensions");
            Assert.AreEqual("D:\\", dir.Path, "Path");
            Assert.AreEqual("SortStrategy", dir.SortStrategy, "SortStrategy");
            Assert.AreEqual(OperationMode.SortOnly, dir.Mode, "Mode");
            Assert.AreEqual(true, dir.IncludeSubDirectories, "IncludeSubDirectories");
        }
    }
}