using System;
using System.IO;
using DonkeySuite.SystemWrappers.Interfaces;

namespace DonkeySuite.SystemWrappers
{
    public class EnvironmentWrapper : IEnvironment
    {
        public bool IsWindowsPlatform
        {
            get { return !(Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX); }
        }

        public string UserHomeDirectory
        {
            get
            {
                return (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME") : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }
        }

        public char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }
    }
}