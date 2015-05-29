using System.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public class DirectoryWrapper : IDirectoryWrapper
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}