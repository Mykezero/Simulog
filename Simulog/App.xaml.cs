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
        public static UnityContainer Container = new UnityContainer();

        public App()
        {
            Container.RegisterInstance<ISettings>(new Settings());
            Container.RegisterType<IViewModel, MainViewModel>();            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Container.Resolve<ISettings>().Load();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Container.Resolve<ISettings>().Save();
        }
    }
}
