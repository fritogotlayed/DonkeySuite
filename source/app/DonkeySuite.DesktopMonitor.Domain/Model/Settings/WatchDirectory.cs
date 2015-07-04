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