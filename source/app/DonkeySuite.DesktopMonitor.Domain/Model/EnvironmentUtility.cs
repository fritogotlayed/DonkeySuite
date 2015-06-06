using MadDonkeySoftware.SystemWrappers;
using MadDonkeySoftware.SystemWrappers.IO;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class EnvironmentUtility : IEnvironmentUtility
    {
        private readonly IEnvironment _environment;
        private readonly IPath _path;

        public EnvironmentUtility(IEnvironment environment, IPath path)
        {
            _environment = environment;
            _path = path;
        }

        public char DirectorySeparatorChar
        {
            get { return _path.DirectorySeparatorChar; }
        }

        public string UserHomeDirectory
        {
            get { return _environment.UserHomeDirectory; }
        }

        public bool IsWindowsPlatform
        {
            get { return _environment.IsWindowsPlatform; }
        }

        public string CombinePath(string path1, string path2)
        {
            return _path.Combine(path1, path2);
        }
    }
}