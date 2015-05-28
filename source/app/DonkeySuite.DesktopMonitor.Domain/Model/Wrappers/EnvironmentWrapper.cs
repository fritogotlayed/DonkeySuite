using System;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public class EnvironmentWrapper : IEnvironmentWrapper
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
    }
}