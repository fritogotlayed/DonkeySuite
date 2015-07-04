using System;
using System.Threading.Tasks;
using DonkeySuite.DesktopMonitor.Domain.Model;
using DonkeySuite.DesktopMonitor.Domain.Model.Settings;
using DonkeySuite.DesktopMonitor.Wpf.Repositories;
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
    public class MainViewModel : DesktopMonitorBaseViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(ILogProvider logProvider) : base(logProvider)
        {
            if (IsInDesignMode)
            {
                // Code runs in Blend --> create design time data.
            }
            else
            {
                // Code runs "for real"
                ScanFolderCommand = new RelayCommand(ScanFolderCommandHandler);
            }
        }

        public RelayCommand ScanFolderCommand { get; set; }

        public void ScanFolderCommandHandler()
        {
            Task.Factory.StartNew(ScanFolder);
        }

        private void ScanFolder()
        {
            using (var session = NHibernateHelper.OpenSession())
            using (var trans = session.BeginTransaction())
                try
                {
                    // TODO: Make this injected.
                    var repo = new WatchedFileRepository(session, DependencyManager.Kernel);
                    var mgr = DependencyManager.Kernel.Get<SettingsManager>();
                    var settings = mgr.GetSettings();
                    foreach (var server in settings.ImageServers)
                    {
                        foreach (var watchDir in server.Directories)
                        {
                            var dir = DependencyManager.Kernel.Get<WatchedDirectory>();
                            dir.Configure(watchDir);
                            dir.ProcessAvailableImages(repo, server);
                        }
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