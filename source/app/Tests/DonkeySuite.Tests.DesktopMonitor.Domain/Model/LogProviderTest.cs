using DonkeySuite.DesktopMonitor.Domain.Model;
using NUnit.Framework;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model
{
    [TestFixture]
    public class LogProviderTest
    {
        private class LogProviderTestBundle
        {
            private LogProvider _logProvider;

            public LogProvider LogProvider
            {
                get { return _logProvider ?? (_logProvider = new LogProvider()); }
            }
        }

        [Test]
        public void LogProviderCreatesObjectForProvidedType()
        {
            // Arrange
            var testBundle = new LogProviderTestBundle();

            // Act
            var log = testBundle.LogProvider.GetLogger(typeof (object));

            // Assert
            Assert.IsNotNull(log);
        }
    }
} 