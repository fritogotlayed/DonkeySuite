/*
<copyright file="ImageServer.cs">
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
using System.Xml.Serialization;
using DonkeySuite.DesktopMonitor.Domain.Model.Providers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class ImageServer : IImageServer
    {
        private readonly IEntityProvider _entityProvider;

        public ImageServer() : this(null)
        {
        }

        public ImageServer(IEntityProvider entityProvider)
        {
            _entityProvider = entityProvider;
        }

        [XmlAttribute("serverUrl")]
        public string ServerUrl { get; set; }

        [XmlArray("WatchDirectories")]
        [XmlArrayItem("dir")]
        public WatchDirectories Directories { get; set; }

        public virtual void PopulateWithDefaults()
        {
            ServerUrl = "http://localhost:8080/DonkeyImageServer";
            Directories = (WatchDirectories) _entityProvider.ProvideDefaultWatchDirectories();
        }
    }
}