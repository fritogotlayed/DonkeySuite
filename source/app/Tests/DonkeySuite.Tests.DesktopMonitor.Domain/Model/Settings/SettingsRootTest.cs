/*
<copyright file="SettingsRootTest.cs">
   Copyright Year MadDonkey Software

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
    [TestFixture]
    public class SettingsRootTest
    {
        private class SettingsRootTestBundle
        {
            public Mock<IEntityProvider> MockServiceLocator { get; private set; }
            public SettingsRoot SettingsRoot { get; private set; }

            public SettingsRootTestBundle()
            {
                MockServiceLocator = new Mock<IEntityProvider>();
                SettingsRoot = new SettingsRoot(MockServiceLocator.Object);
            }
        }

        [TearDown]
        public void TearDown()
        {
            GC.Collect();
        }

        [Test]
        public void SettingsInitializedNull()
        {
            // Act
            var testBundle = new SettingsRootTestBundle();

            // Assert
            Assert.IsNull(testBundle.SettingsRoot.ImageServers);
        }

        [Test]
        public void SettingsConfiguredProperly()
        {
            // Arrange
            var testBundle = new SettingsRootTestBundle();
            var mockImageServers = new Mock<ImageServers>();

            testBundle.MockServiceLocator.Setup(x => x.ProvideDefaultImageServers()).Returns(mockImageServers.Object);

            // Act
            testBundle.SettingsRoot.PopulateWithDefaults();

            // Assert
            Assert.IsNotNull(testBundle.SettingsRoot.ImageServers);
        }

        [Test]
        public void SettingsSetsWorkCorrectly()
        {
            // Arrange
            var testBundle = new SettingsRootTestBundle();

            var mockImageServers = new Mock<ImageServers>();

            // Act
            testBundle.SettingsRoot.ImageServers = mockImageServers.Object;

            // Assert
            Assert.AreSame(mockImageServers.Object, testBundle.SettingsRoot.ImageServers);
        }
    }
}