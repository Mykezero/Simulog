using Microsoft.Practices.Unity;
using Simulog.Model;
using Simulog.ViewModel;
using System.Windows;

namespace Simulog
{
    /// <summary>
    /// Handles and sets up class dependencies. 
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Our dependency injection container to resolve our dependencies. 
        /// </summary>
        public static UnityContainer Container = new UnityContainer();

        /// <summary>
        /// Map our objects with their interfaces so that we can resolve 
        /// our dependancies. 
        /// </summary>
        public App()
        {
            Container.RegisterInstance<ISettings>(new Settings());
            Container.RegisterType<IViewModel, MainViewModel>();            
        }

        /// <summary>
        /// Load settings on startup. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            Container.Resolve<ISettings>().Load();
        }

        /// <summary>
        /// Save settings on exit. 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<ISettings>().Save();
        }
    }
}
