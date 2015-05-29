using System.Xml.Serialization;
using DonkeySuite.SystemWrappers.Interfaces;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Settings
{
    public class WatchDirectory
    {
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

        public void PopulateWithDefaults()
        {
            var environment = DependencyManager.Kernel.Get<IEnvironment>();
            Path = environment.IsWindowsPlatform ? "C:\\" : "/"; // TODO: Make platform independent
            FileExtensions = "jpg,jpeg,gif,tiff";
            Mode = OperationMode.Unknown;
            SortStrategy = "Simple";
        }
    }
}