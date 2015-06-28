using System.Windows.Media;
using DonkeySuite.DesktopMonitor.Domain.Model;

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
    public class SettingsViewModel : DesktopMonitorBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SettingsViewModel(ILogProvider logProvider) : base(logProvider)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
                Background = new SolidColorBrush(Colors.White);
            }
            else
            {
                // Code runs "for real"
            }
        }

        public Brush Background { get; set; }
    }
}