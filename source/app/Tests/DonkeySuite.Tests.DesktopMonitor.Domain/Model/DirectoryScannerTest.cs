/*
<copyright file="DirectoryScannerTest.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Repositories;
using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using log4net;
using MadDonkeySoftware.SystemWrappers.IO;
using Moq;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model
{
    public class DirectoryScannerTest
    {
        private class DirectoryScannerTestBundle
        {
            private DirectoryScanner _directoryScanner;

            public Mock<ILogProvider> MockLogProvider { get; private set; }
            public Mock<IPath> MockPath { get; private set; }
            public Mock<IDirectory> MockDirectory { get; private set; }

            public DirectoryScanner DirectoryScanner
            {
                get { return _directoryScanner ?? (_directoryScanner = new DirectoryScanner(MockLogProvider.Object, MockPath.Object, MockDirectory.Object)); }
            }

            public DirectoryScannerTestBundle()
            {
                MockLogProvider = new Mock<ILogProvider>();
                MockPath = new Mock<IPath>();
                MockDirectory = new Mock<IDirectory>();
            }
        }

        [Test]
        public void DirectoryScannerGetAvailableImagesReturnsImagesForTopLevelDirectoryCorrectly()
        {
            // Arrange
            var testBundle = new DirectoryScannerTestBundle();
            var mockWatchFileRepository = new Mock<IWatchedFileRepository>();
            var mockExistingWatchedFile = new Mock<IWatchedFile>();
            var mockNewWatchedFile = new Mock<IWatchedFile>();
            var mockSortStrategy = new Mock<ISortStrategy>();
            var mockLog = new Mock<ILog>();

            testBundle.MockDirectory.Setup(x => x.GetFiles(It.IsAny<string>())).Returns(new[] {"test.abc", "test2.abc"});
            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);
            testBundle.MockPath.Setup(x => x.GetExtension(It.Is<string>(y => y.EndsWith(".abc")))).Returns("abc");

            mockWatchFileRepository.Setup(x => x.LoadFileForPath(It.IsAny<string>())).Returns(mockExistingWatchedFile.Object);
            mockWatchFileRepository.Setup(x => x.CreateNew()).Returns(mockNewWatchedFile.Object);

            // Act
            var rslt = testBundle.DirectoryScanner.GetAvailableImages(mockWatchFileRepository.Object, string.Empty, new[] {"abc"}, false, mockSortStrategy.Object);

            // Assert
            Assert.IsNotNull(rslt);
            Assert.AreEqual(2, rslt.Count);
            mockWatchFileRepository.Verify(x => x.LoadFileForPath(It.IsAny<string>()), Times.Exactly(2));
            mockWatchFileRepository.Verify(x => x.CreateNew(), Times.Exactly(2));
            mockWatchFileRepository.Verify(x => x.Save(It.IsAny<WatchedFile>()), Times.Never());
        }

        [Test]
        public void DirectoryScannerGetAvailableImagesForSubDirectoriesCorrectly()
        {
            // Arrange
            var testBundle = new DirectoryScannerTestBundle();
            var mockWatchFileRepository = new Mock<IWatchedFileRepository>();
            var mockNewWatchedFile = new Mock<IWatchedFile>();
            var mockSortStrategy = new Mock<ISortStrategy>();
            var mockLog = new Mock<ILog>();

            testBundle.MockDirectory.Setup(x => x.GetFiles(string.Empty)).Returns(new[] {"test.abc"});
            testBundle.MockDirectory.Setup(x => x.GetFiles("test")).Returns(new[] {"test2.abc"});
            testBundle.MockDirectory.Setup(x => x.GetDirectories(string.Empty)).Returns(new[] {"test"});
            testBundle.MockLogProvider.Setup(x => x.GetLogger(It.IsAny<Type>())).Returns(mockLog.Object);
            testBundle.MockPath.Setup(x => x.GetExtension(It.Is<string>(y => y.EndsWith(".abc")))).Returns("abc");

            mockWatchFileRepository.Setup(x => x.CreateNew()).Returns(mockNewWatchedFile.Object);

            // Act
            var rslt = testBundle.DirectoryScanner.GetAvailableImages(mockWatchFileRepository.Object, string.Empty, new[] {"abc"}, true, mockSortStrategy.Object);

            // Assert
            Assert.IsNotNull(rslt);
            Assert.AreEqual(2, rslt.Count);
            mockWatchFileRepository.Verify(x => x.LoadFileForPath(It.IsAny<string>()), Times.Exactly(2));
            mockWatchFileRepository.Verify(x => x.CreateNew(), Times.Exactly(2));
            mockWatchFileRepository.Verify(x => x.Save(It.IsAny<WatchedFile>()), Times.Never());
        }
    }
}