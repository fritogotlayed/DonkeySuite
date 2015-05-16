using System;

namespace DonkeySuite.Tests.DesktopMonitor.Domain
{
    public static class Utilities
    {
        public static bool IsWindowsPlatform
        {
            get
            {
                return !(Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX);
            }
        }
    }
}