/*
<copyright file="ImageServerTest.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServerTest
    {
        private class ImageServerTestBundle
        {
            public ImageServer ImageServer { get; private set; }
            public Mock<IEntityProvider> MockServiceProvider { get; private set; }

            public ImageServerTestBundle()
            {
                MockServiceProvider = new Mock<IEntityProvider>();
                ImageServer = new ImageServer(MockServiceProvider.Object);
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void ImageServerInitializedNull()
        {
            // Arrange
            var testBundle = new ImageServerTestBundle();

            // Act
            var url = testBundle.ImageServer.ServerUrl;

            // Assert
            Assert.AreEqual(null, url);
        }

        [Test]
        public void ImageServerConfiguredProperly()
        {
            // Arrange
            var testBundle = new ImageServerTestBundle();
            var mock = new  Mock<WatchDirectories>();

            testBundle.MockServiceProvider.Setup(x => x.ProvideDefaultWatchDirectories()).Returns(mock.Object);

            // Act
            testBundle.ImageServer.PopulateWithDefaults();

            // Assert
            Assert.AreEqual("http://localhost:8080/DonkeyImageServer", testBundle.ImageServer.ServerUrl);
        }

        [Test]
        public void ImageServerSetsWorkCorrectly()
        {
            // Arrange
            var testBundle = new ImageServerTestBundle();
            var url = "http://localhost:9000/Blah";

            // Act
            testBundle.ImageServer.ServerUrl = url;

            // Assert
            Assert.AreEqual(url, testBundle.ImageServer.ServerUrl);
        }

    }
}