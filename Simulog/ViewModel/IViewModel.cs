using Simulog.Model;
using System.Windows.Input;

namespace Simulog.ViewModel
{
    /// <summary>
    /// The properties from which the main UI gets its 
    /// commands and data.
    /// </summary>
    public interface IViewModel
    {
        ISettings Settings { get; set; }
        ICommand LoginCommand { get; set; }
        ICommand SetConfigurationCommand { get; set; }
    }
}
