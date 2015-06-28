using DonkeySuite.DesktopMonitor.Domain.Model;
using GalaSoft.MvvmLight;
using log4net;

namespace DonkeySuite.DesktopMonitor.Wpf.ViewModel
{
    public abstract class DesktopMonitorBaseViewModel : ViewModelBase
    {

        protected DesktopMonitorBaseViewModel(ILogProvider logProvider)
        {
            if (!IsInDesignMode)
            {
                Log = logProvider.GetLogger(GetType());
            }
        }

        protected ILog Log { get; private set; }
    }
}