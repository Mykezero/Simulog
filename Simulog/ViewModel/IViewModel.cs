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
        /// <summary>
        /// The settings of the view model. 
        /// </summary>
        ISettings Settings { get; set; }

        /// <summary>
        /// A command to log the accounts in with. 
        /// </summary>
        ICommand LoginCommand { get; set; }

        /// <summary>
        /// A command to set the configuration path. 
        /// </summary>
        ICommand SetConfigurationCommand { get; set; }
    }
}
