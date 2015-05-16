using System;
using System.Collections;
using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Domain.Model.Wrappers;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class WatchDirectoriesTest
    {
        [SetUp]
        public void SetUp()
        {
            DependencyManager.Kernel = null;
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void WatchDirectoriesInitializedNull()
        {
            // Act
            var dirs = new WatchDirectories();

            // Assert
            Assert.AreEqual(new List<WatchDirectory>(), dirs);
        }

        [Test]
        public void WatchDirectoriesConfiguredProperly()
        {
            // Arrange
            WatchDirectories dirs = new WatchDirectories();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();

            DependencyManager.Kernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);

            // Act
            dirs.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(dirs);
            Assert.AreEqual(1, dirs.Count);
        }

        [Test]
        public void WatchDirectoriesEnumeratorIsCorrectForGeneric()
        {
            // Arrange
            var dir = new WatchDirectories();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();

            DependencyManager.Kernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            dir.PopulateWithDefaults();

            // Act
            var it = dir.GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }

        [Test]
        public void WatchDirectoriesEnumeratorIsCorrectForNonGeneric()
        {
            // Arrange
            var dir = new WatchDirectories();
            var mockEnvironmentWrapper = new Mock<IEnvironmentWrapper>();

            DependencyManager.Kernel.Bind<IEnvironmentWrapper>().ToMethod(context => mockEnvironmentWrapper.Object);
            dir.PopulateWithDefaults();

            // Act
            var it = ((IEnumerable)dir).GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }
    }
}