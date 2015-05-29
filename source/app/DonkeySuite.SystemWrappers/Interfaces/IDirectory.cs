using System.IO;

namespace DonkeySuite.SystemWrappers.Interfaces
{
    public interface IDirectory
    {
        DirectoryInfo CreateDirectory(string path);
    }
}