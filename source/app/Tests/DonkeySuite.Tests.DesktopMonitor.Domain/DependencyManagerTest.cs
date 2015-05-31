using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using Ninject;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain
{
    /*
    [TestFixture]
    public class DependencyManagerTest
    {
         
        [Test]
        public void DependencyManagerGetSettingsManagerInstanceIsNotNull()
        {
            DependencyManager.Kernel = null;

            Assert.IsNotNull(DependencyManager.Kernel.Get<SettingsManager>());
        }

        [Test]
        public void DependencyManagerGetSettingsManagerMultipleCallsToInstanceReturnsTheSameObject()
        {
            // Arrange
            DependencyManager.Kernel = null;

            // Act
            var m1 = DependencyManager.Kernel.Get<SettingsManager>();
            var m2 = DependencyManager.Kernel.Get<SettingsManager>();

            // Assert
            Assert.AreSame(m1, m2);
        }
    }
     */
}