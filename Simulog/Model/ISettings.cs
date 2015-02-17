using System.Collections.ObjectModel;

namespace Simulog.Model
{
    /// <summary>
    /// Interface for controlling application data. 
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// The starting file dialog directory. 
        /// </summary>
        string InitialDirectory { get; set; }

        /// <summary>
        /// Path to the configuration xml. 
        /// </summary>
        string ConfigurationPath { get; set; }

        /// <summary>
        /// Path to the configuration xml. 
        /// </summary>
        string ClientPath{ get; set; }
        
        /// <summary>
        /// Server to log-on to. 
        /// </summary>
        string ServerIP { get; set; }
        
        /// <summary>
        /// Player account details. 
        /// </summary>
        ObservableCollection<Account> Accounts { get; set; }
        
        /// <summary>
        /// Save settings to file. 
        /// </summary>
        void Save();
        
        /// <summary>
        /// Load settings from file. 
        /// </summary>
        void Load();
    }
}
