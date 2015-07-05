/*
<copyright file="MainViewModel.cs">
   Copyright 2015 MadDonkey Software

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
</copyright>
*/
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