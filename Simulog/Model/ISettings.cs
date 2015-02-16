using System.Collections.ObjectModel;

namespace Simulog.Model
{
    /// <summary>
    /// Interface for controlling application data. 
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Path to the configuration xml. 
        /// </summary>
        string ConfigurationFilePath { get; set; }
        
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
