/*
<copyright file="WatchDirectory.cs">
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
using MadDonkeySoftware.SystemWrappers;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class WatchDirectory : IWatchDirectory
    {
        private readonly IEnvironment _environment;

        public WatchDirectory() : this(null)
        {
        }

        public WatchDirectory(IEnvironment environment)
        {
            _environment = environment;
        }

        [XmlAttribute("path")]
        public string Path { get; set; }

        [XmlAttribute("includeSubDirectories")]
        public bool IncludeSubDirectories { get; set; }

        [XmlAttribute("fileExtensions")]
        public string FileExtensions { get; set; }

        [XmlAttribute("mode")]
        public OperationMode Mode { get; set; }

        [XmlAttribute("sortStrategy")]
        public string SortStrategy { get; set; }

        public virtual void PopulateWithDefaults()
        {
            Path = _environment.IsWindowsPlatform ? "C:\\" : "/";
            FileExtensions = "jpg,jpeg,gif,tiff,png";
            Mode = OperationMode.Unknown;
            SortStrategy = "Simple";
        }
    }
}