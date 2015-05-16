using System;

namespace DonkeySuite.DesktopMonitor.Domain.Model.Wrappers
{
    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        public bool IsWindowsPlatform
        {
            get { return !(Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX); }
        }
    }
}