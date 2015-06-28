/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:DonkeySuite.DesktopMonitor.Wpf"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

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

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register(() => DependencyManager.Kernel.Get<MainViewModel>());
            SimpleIoc.Default.Register(() => DependencyManager.Kernel.Get<MainWindowViewModel>());
            SimpleIoc.Default.Register(() => DependencyManager.Kernel.Get<SettingsViewModel>());
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