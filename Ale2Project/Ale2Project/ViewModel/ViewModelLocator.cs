/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocatorTemplate xmlns:vm="clr-namespace:Ale2Project.ViewModel"
                                   x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"
*/

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Ale2Project.Model;
using Ale2Project.Service;

namespace Ale2Project.ViewModel
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

            //ViewModels
            SimpleIoc.Default.Register<MainViewModel>();

            //Services
            SimpleIoc.Default.Register<IFileService, FileService>();
            SimpleIoc.Default.Register<IGraphVizService, GraphVizService>();
            SimpleIoc.Default.Register<IDfaCheckService, DfaCheckService>();
            SimpleIoc.Default.Register<IAcceptedStringCheckService, AcceptedStringCheckService>();
            SimpleIoc.Default.Register<IRegularExpressionParserService, RegularExpressionParserService>();
        }

        /// <summary>
        /// Gets the Main property.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
             "CA1822:MarkMembersAsStatic",
             Justification = "This non-static member is needed for data binding purposes.")]
        public MainViewModel MainViewModel
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public IFileService FileService
        {
            get { return ServiceLocator.Current.GetInstance<IFileService>(); }
        }

        public IGraphVizService GraphVizService
        {
            get { return ServiceLocator.Current.GetInstance<IGraphVizService>(); }
        }

        public IDfaCheckService DfaCheckService
        {
            get { return ServiceLocator.Current.GetInstance<IDfaCheckService>(); }
        }

        public IAcceptedStringCheckService AcceptedStringCheckService
        {
            get { return ServiceLocator.Current.GetInstance<IAcceptedStringCheckService>(); }
        }

        public IRegularExpressionParserService RegularExpressionParserService
        {
            get { return ServiceLocator.Current.GetInstance<IRegularExpressionParserService>(); }
        }

        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}