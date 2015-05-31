using System.IO;

namespace MadDonkeySoftware.SystemWrappers.IO
{
    public class PathWrapper : IPath
    {
        public char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }

        public string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }
    }
}