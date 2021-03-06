/*
<copyright file="WatchDirectoriesTest.cs">
   Copyright 2015 MadDonkey Software

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
</copyright>
*/
using System;
using System.Collections;
using System.Collections.Generic;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    [TestFixture]
    public class WatchDirectoriesTest
    {
        private class WatchDirectoryTestBundle
        {
            public Mock<IEntityProvider> MockServiceLocator { get; private set; }
            public WatchDirectories WatchDirectories { get; private set; }

            public WatchDirectoryTestBundle()
            {
                MockServiceLocator = new Mock<IEntityProvider>();
                WatchDirectories = new WatchDirectories(MockServiceLocator.Object);
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void WatchDirectoriesConstructedWithNull()
        {
            // Arrange
            var watchDirectories = new WatchDirectories();
            NullReferenceException expectedException = null;

            try
            {
                watchDirectories.PopulateWithDefaults();
            }
            catch (NullReferenceException ex)
            {
                expectedException = ex;
            }

            Assert.IsNotNull(expectedException);
        }

        [Test]
        public void WatchDirectoriesInitializedNull()
        {
            // Arrange & Act
            var testBundle = new WatchDirectoryTestBundle();

            // Assert
            Assert.AreEqual(new List<WatchDirectory>(), testBundle.WatchDirectories);
        }

        [Test]
        public void WatchDirectoriesConfiguredProperly()
        {
            // Arrange
            var testBundle = new WatchDirectoryTestBundle();
            var mockWatchDirectory = new Mock<WatchDirectory>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultWatchDirectory()).Returns(mockWatchDirectory.Object);

            // Act
            testBundle.WatchDirectories.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(testBundle.WatchDirectories);
            Assert.AreEqual(1, testBundle.WatchDirectories.Count);
        }

        [Test]
        public void WatchDirectoriesEnumeratorIsCorrectForGeneric()
        {
            // Arrange
            var testBundle = new WatchDirectoryTestBundle();
            var mockWatchDirectory = new Mock<WatchDirectory>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultWatchDirectory()).Returns(mockWatchDirectory.Object);
            testBundle.WatchDirectories.PopulateWithDefaults();

            // Act
            var it = testBundle.WatchDirectories.GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }

        [Test]
        public void WatchDirectoriesEnumeratorIsCorrectForNonGeneric()
        {
            // Arrange
            var testBundle = new WatchDirectoryTestBundle();
            var mockWatchDirectory = new Mock<WatchDirectory>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultWatchDirectory()).Returns(mockWatchDirectory.Object);
            testBundle.WatchDirectories.PopulateWithDefaults();

            // Act
            var it = ((IEnumerable)testBundle.WatchDirectories).GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }
    }
}