/*
<copyright file="LogProviderTest.cs">
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