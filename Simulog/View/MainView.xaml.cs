using Microsoft.Practices.Unity;
using Simulog.ViewModel;
using System.Windows;

namespace Simulog.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.DataContext = App.Container.Resolve<IViewModel>();
        }
    }
}
