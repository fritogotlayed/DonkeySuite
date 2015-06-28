using System.Windows.Input;
using DonkeySuite.DesktopMonitor.Domain.Model;
using GalaSoft.MvvmLight.Command;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Wpf.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainWindowViewModel : DesktopMonitorBaseViewModel
    {
        private DesktopMonitorBaseViewModel _currentViewModel;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainWindowViewModel(ILogProvider logProvider) : base(logProvider)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                CurrentViewModel = DependencyManager.Kernel.Get<MainViewModel>();
            }
        }

        public DesktopMonitorBaseViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                RaisePropertyChanged();
            }
        }

        // Adapted from http://stackoverflow.com/questions/19654295/wpf-mvvm-navigate-views
        public ICommand DisplayTestView
        {
            get { return new RelayCommand(() => CurrentViewModel = DependencyManager.Kernel.Get<SettingsViewModel>(), () => true); }
        }

        public ICommand DisplayMainView
        {
            get { return new RelayCommand(() => CurrentViewModel = DependencyManager.Kernel.Get<MainViewModel>(), () => true); }
        }
    }
}