using System;
using log4net;

namespace DonkeySuite.DesktopMonitor.Domain.Model
{
    public class LogProvider : ILogProvider
    {
        public ILog GetLogger(Type type)
        {
            return LogManager.GetLogger(type);
        }
    }
}