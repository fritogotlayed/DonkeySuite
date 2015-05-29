using System.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public interface IDirectoryWrapper
    {
        DirectoryInfo CreateDirectory(string path);
    }
}