/*
<copyright file="WebRequestFactoryTests.cs">
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
using NUnit.Framework;
using MadDonkeySoftware.SystemWrappers.Net;

namespace DonkeySuite.Tests.DesktopMonitor.Domain.Model.Requests
{
    public class WebRequestFactoryTests
    {
        [Test]
        public void WebRequestFactory_CreateWebRequest_CreatesANewWebRequestEachCall()
        {
            // Arrange
            var factory = new WebRequestFactory();

            // Act
            var request1 = factory.Create("http://localhost");
            var request2 = factory.Create("http://localhost");

            // Assert
            Assert.IsNotNull(request1);
            Assert.IsNotNull(request2);
            Assert.AreNotSame(request1, request2);
        }
    }
}