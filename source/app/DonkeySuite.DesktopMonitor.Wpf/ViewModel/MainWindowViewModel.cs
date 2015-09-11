/*
<copyright file="MainWindowViewModel.cs">
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
using DonkeySuite.DesktopMonitor.Domain.Model;
using GalaSoft.MvvmLight.Messaging;

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
                Messenger.Default.Register<NavigateToViewMessage>(this, ReceiveMessage);
                CurrentViewModel = (DesktopMonitorBaseViewModel) DependencyManager.Kernel.GetService(typeof(MainViewModel));
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

        private void ReceiveMessage(NavigateToViewMessage action)
        {
            CurrentViewModel = (DesktopMonitorBaseViewModel) DependencyManager.Kernel.GetService(action.ViewType);
        }
    }
}