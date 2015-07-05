/*
<copyright file="ViewModelLocator.cs">
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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Ninject;

namespace DonkeySuite.DesktopMonitor.Wpf.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
                // SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }
            else
            {
                // Create run time view services and models
                // SimpleIoc.Default.Register<IDataService, DataService>();
                SimpleIoc.Default.Register(() => DependencyManager.Kernel.Get<MainViewModel>());
                SimpleIoc.Default.Register(() => DependencyManager.Kernel.Get<MainWindowViewModel>());
                SimpleIoc.Default.Register(() => DependencyManager.Kernel.Get<SettingsViewModel>());
            }
        }

        public MainViewModel MainViewModel { get { return ServiceLocator.Current.GetInstance<MainViewModel>(); } }

        public MainWindowViewModel MainWindowViewModel { get { return ServiceLocator.Current.GetInstance<MainWindowViewModel>(); } }

        public SettingsViewModel TestViewModel { get { return ServiceLocator.Current.GetInstance<SettingsViewModel>(); } }

        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}