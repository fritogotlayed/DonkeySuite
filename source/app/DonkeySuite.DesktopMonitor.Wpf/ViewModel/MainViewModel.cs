using System;
using System.Threading.Tasks;
using DonkeySuite.DesktopMonitor.Domain;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Wpf.Repositories;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            Task.Factory.StartNew(ScanFolder);
        }

        private void ScanFolder()
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var trans = session.BeginTransaction())
                try
                {
                    // TODO: Figure out how to make this injected.
                    var repo = new WatchedFileRepository(session, DependencyManager.Kernel);
                    var mgr = DependencyManager.Kernel.Get<SettingsManager>();
                    var settings = mgr.GetSettings();
                    foreach (var watchDir in settings.Directories)
                    {
                        var dir = DependencyManager.Kernel.Get<WatchedDirectory>();
                        dir.Configure(watchDir);
                        dir.ProcessAvailableImages(repo);
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    if (trans != null && trans.IsActive)
                    {
                        trans.Rollback();
                    }

                    Log.Error("Failed to process images. ", ex);
                }
                finally
                {
                    if (session != null)
                    {
                        session.Close();
                    }
                }
        }
    }
}