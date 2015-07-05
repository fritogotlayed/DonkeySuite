/*
<copyright file="ImageServersTest.cs">
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
    public class ImageServersTest
    {
        private class ImageServersTestBundle
        {
            public Mock<IEntityProvider> MockServiceLocator { get; private set; }
            public ImageServers ImageServers { get; private set; }

            public ImageServersTestBundle()
            {
                MockServiceLocator = new Mock<IEntityProvider>();
                ImageServers = new ImageServers(MockServiceLocator.Object);
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
            var watchDirectories = new ImageServers();
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
        public void ImageServersInitializedNull()
        {
            // Arrange & Act
            var testBundle = new ImageServersTestBundle();

            // Assert
            Assert.AreEqual(new List<WatchDirectory>(), testBundle.ImageServers);
        }

        [Test]
        public void ImageServersConfiguredProperly()
        {
            // Arrange
            var testBundle = new ImageServersTestBundle();
            var mockImageServer = new Mock<ImageServer>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServer()).Returns(mockImageServer.Object);

            // Act
            testBundle.ImageServers.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(testBundle.ImageServers);
            Assert.AreEqual(1, testBundle.ImageServers.Count);
        }

        [Test]
        public void ImageServersEnumeratorIsCorrectForGeneric()
        {
            // Arrange
            var testBundle = new ImageServersTestBundle();
            var mockImageServer = new Mock<ImageServer>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServer()).Returns(mockImageServer.Object);
            testBundle.ImageServers.PopulateWithDefaults();

            // Act
            var it = testBundle.ImageServers.GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }

        [Test]
        public void ImageServersEnumeratorIsCorrectForNonGeneric()
        {
            // Arrange
            var testBundle = new ImageServersTestBundle();
            var mockImageServer = new Mock<ImageServer>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServer()).Returns(mockImageServer.Object);
            testBundle.ImageServers.PopulateWithDefaults();

            // Act
            var it = ((IEnumerable)testBundle.ImageServers).GetEnumerator();

            // Assert
            Assert.AreEqual(true, it.MoveNext());
            Assert.IsNotNull(it.Current);
            Assert.AreEqual(false, it.MoveNext());
        }
    }
}