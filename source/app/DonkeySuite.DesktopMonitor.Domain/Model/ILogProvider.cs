using System;
using log4net;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public interface ILogProvider
    {
        ILog GetLogger(Type type);
    }
}