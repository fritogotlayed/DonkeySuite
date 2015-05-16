using System.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public class FileWrapper : IFileWrapper
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public FileStream Open(string path, FileMode mode)
        {
            return File.Open(path, mode);
        }

        public byte[] ReadAllBytes(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}