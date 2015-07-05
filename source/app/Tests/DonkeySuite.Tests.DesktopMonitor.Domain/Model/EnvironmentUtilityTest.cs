/*
<copyright file="EnvironmentUtilityTest.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model;
using MadDonkeySoftware.SystemWrappers;
using MadDonkeySoftware.SystemWrappers.IO;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model
{
    [TestFixture]
    public class EnvironmentUtilityTest
    {
        private class EnvironmentUtilityTestBundle
        {
            private EnvironmentUtility _environmentProvider;

            public Mock<IEnvironment> MockEnvironment { get; set; }
            public Mock<IPath> MockPath { get; set; }

            public EnvironmentUtility EnvironmentUtility
            {
                get { return _environmentProvider ?? (_environmentProvider = new EnvironmentUtility(MockEnvironment.Object, MockPath.Object)); }
            }

            public EnvironmentUtilityTestBundle()
            {
                MockEnvironment = new Mock<IEnvironment>();
                MockPath = new Mock<IPath>();
            }
        }

        [Test]
        public void EnvironmentUtilityDirectorySeperatorReturnsValueFromPath()
        {
            // Arrange
            var testBundle = new EnvironmentUtilityTestBundle();

            // Act
            var val = testBundle.EnvironmentUtility.DirectorySeparatorChar;

            // Assert
            Assert.IsNotNull(val);
            testBundle.MockPath.VerifyGet(x => x.DirectorySeparatorChar, Times.Once());
        }

        [Test]
        public void EnvironmentUtilityCombinePathReturnsValueFromPathCombine()
        {
            // Arrange
            var testBundle = new EnvironmentUtilityTestBundle();
            const string path1 = "path1";
            const string path2 = "path2";
            const string combineResult = "path1/path2";

            testBundle.MockPath.Setup(x => x.Combine(path1, path2)).Returns(combineResult);

            // Act
            var val = testBundle.EnvironmentUtility.CombinePath(path1, path2);

            // Assert
            Assert.AreSame(combineResult, val);
            testBundle.MockPath.Verify(x => x.Combine(path1, path2), Times.Once());
        }

        [Test]
        public void EnvironmentUtilityIsWindowsPlatformReturnsValueFromEnvironment()
        {
            // Arrange
            var testBundle = new EnvironmentUtilityTestBundle();

            // Act
            var val = testBundle.EnvironmentUtility.IsWindowsPlatform;

            // Assert
            Assert.IsFalse(val);
            testBundle.MockEnvironment.VerifyGet(x => x.IsWindowsPlatform, Times.Once());
        }

        [Test]
        public void EnvironmentUtilityUserHomeDirectoryReturnsValueFromEnvironment()
        {
            // Arrange
            var testBundle = new EnvironmentUtilityTestBundle();

            // Act
            var val = testBundle.EnvironmentUtility.UserHomeDirectory;

            // Assert
            Assert.IsNull(val);
            testBundle.MockEnvironment.VerifyGet(x => x.UserHomeDirectory, Times.Once());
        }
    }
} 