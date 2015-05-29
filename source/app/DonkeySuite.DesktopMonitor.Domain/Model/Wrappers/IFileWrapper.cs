using System.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public interface IFileWrapper
    {
        bool Exists(string path);
        FileStream Open(string path, FileMode mode);
        byte[] ReadAllBytes(string path);
        void Move(string oldPath, string newPath);
    }
}