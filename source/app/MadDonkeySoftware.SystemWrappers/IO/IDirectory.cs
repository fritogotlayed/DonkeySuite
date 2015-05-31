using System.IO;

namespace MadDonkeySoftware.SystemWrappers.IO
{
    public interface IDirectory
    {
        DirectoryInfo CreateDirectory(string path);
    }
}