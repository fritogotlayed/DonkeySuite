using DonkeySuite.DesktopMonitor.Domain.Model;
using FluentNHibernate.Mapping;

namespace DonkeySuite.DesktopMonitor.Wpf.Mappings
{
    public class WatchedFileMap : ClassMap<WatchedFile>
    {
        public WatchedFileMap()
        {
            Id(x => x.FullPath).GeneratedBy.Assigned();
            Map(x => x.FileName).Not.Nullable();
            Map(x => x.UploadSuccessful).Not.Nullable();
        }
    }
}