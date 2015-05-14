using DonkeySuite.DesktopMonitor.Domain.Model.Requests;
using GalaSoft.MvvmLight;
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
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                TestRequest = new RelayCommand(TestRequestCommandHandler);
            }
        }

        public RelayCommand TestRequest { get; set; }

        public void TestRequestCommandHandler()
        {
            var req = DependencyManager.Kernel.Get<AddImageRequest>();
            req.FileName = "Test.txt";
            req.FileBytes = new byte[] {2, 3, 4, 5, 6, 7, 7};
            req.RequestUrl = "http://localhost:61788";
            req.Post();
        }
    }
}