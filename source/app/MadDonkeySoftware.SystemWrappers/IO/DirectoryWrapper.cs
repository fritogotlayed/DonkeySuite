using System.IO;

namespace MadDonkeySoftware.SystemWrappers.IO
{
    public class DirectoryWrapper : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}