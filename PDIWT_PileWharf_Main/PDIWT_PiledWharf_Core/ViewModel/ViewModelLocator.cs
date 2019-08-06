/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:PDIWT_PiledWharf_Core.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using PDIWT_PiledWharf_Core.Model;

namespace PDIWT_PiledWharf_Core.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// <para>
    /// See http://www.mvvmlight.net
    /// </para>
    /// </summary>
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<Input_ImportFromFileViewModel>();
            SimpleIoc.Default.Register<CurrentForceViewModel>();
            SimpleIoc.Default.Register<ReportGeneratorViewModel>();
            SimpleIoc.Default.Register<PilePlacementViewModel>();
            SimpleIoc.Default.Register<WaveForceViewModel>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]
        public SettingsViewModel Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SettingsViewModel>();
            }
        }

        /// <summary>
        /// Gets the Input's ViewModel.
        /// </summary>
        public Input_ImportFromFileViewModel Input_ImportFromFile
        {
            get
            {
                return ServiceLocator.Current.GetInstance<Input_ImportFromFileViewModel>();
            }
        }

        public CurrentForceViewModel CurrentForce
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CurrentForceViewModel>();
            }
        }

        public ReportGeneratorViewModel ReportGenerator => ServiceLocator.Current.GetInstance<ReportGeneratorViewModel>();

        public PilePlacementViewModel PilePlacement => ServiceLocator.Current.GetInstance<PilePlacementViewModel>();

        public WaveForceViewModel WaveForce => ServiceLocator.Current.GetInstance<WaveForceViewModel>();

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}