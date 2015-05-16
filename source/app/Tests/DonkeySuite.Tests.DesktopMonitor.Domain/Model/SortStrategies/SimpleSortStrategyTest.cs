using DonkeySuite.DesktopMonitor.Domain.Model.SortStrategies;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.SortStrategies
{
    [TestFixture]
    public class SimpleSortStrategyTest
    {
        [Test]
        public void SimpleSortStrategy_NewFileName_FirstLetterOfFileNameUsedForSubDirectory()
        {
            // Arrange
            var strategy = new SimpleSortStrategy();
            var dir = Utilities.IsWindowsPlatform ? @"C:\folder" : "/folder";

            // Act
            var newName = strategy.NewFileName(dir, "test.txt");

            // Assert
            var expected = Utilities.IsWindowsPlatform ? @"C:\folder\t\test.txt" : "/folder/t/test.txt";

            Assert.AreEqual(expected, newName);
        }
    }
}