using System.IO;
using DonkeySuite.SystemWrappers.Interfaces;

namespace DonkeySuite.SystemWrappers
{
    public class DirectoryWrapper : IDirectory
    {
        public DirectoryInfo CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path);
        }
    }
}