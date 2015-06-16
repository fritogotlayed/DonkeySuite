using System.IO;

namespace MadDonkeySoftware.SystemWrappers.IO
{
    public class DirectoryWrapper : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}