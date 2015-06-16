using System.IO;

namespace MadDonkeySoftware.SystemWrappers.IO
{
    public interface IDirectory
    {
        DirectoryInfo CreateDirectory(string path);
        string[] GetFiles(string path);
        string[] GetDirectories(string path);
    }
}