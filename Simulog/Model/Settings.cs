using Microsoft.Practices.Prism.Mvvm;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Simulog.Model
{
    /// <summary>
    /// Handles access to the application data. 
    /// </summary>
    public class Settings : BindableBase, ISettings
    {
        /// <summary>
        /// The name of the file where all our settings are going to be stored. 
        /// </summary>
        private static readonly string SETTINGS_FILE = "settings.xml";

        private string _serverIP;

        /// <summary>
        /// The IP of the server we are login accounts onto. 
        /// </summary>
        public string ServerIP
        {
            get { return _serverIP; }
            set { SetProperty(ref _serverIP, value); }
        }

        private string initialDirectory;

        /// <summary>
        /// The initial directory to look for clients / configuration files in. 
        /// </summary>
        public string InitialDirectory
        {
            get { return initialDirectory; }
            set { initialDirectory = value; }
        }

        private string _configurationPath;
        
        /// <summary>
        /// Path the configuration path with the user's desired GUI settings. 
        /// </summary>
        public string ConfigurationPath
        {
            get { return _configurationPath; }
            set { SetProperty(ref _configurationPath, value); }
        }

        private string _clientPath;

        /// <summary>
        /// Path the Ashita Client. 
        /// </summary>
        public string ClientPath
        {
            get { return _clientPath; }
            set { SetProperty(ref _clientPath, value); }
        }

        /// <summary>
        /// List of accounts to log in. 
        /// </summary>
        public ObservableCollection<Account> Accounts { get; set; }

        public Settings()
        {
            this.ServerIP = "127.0.0.1";
            this.InitialDirectory = System.Environment.CurrentDirectory;
            this.ConfigurationPath = string.Empty;
            this.Accounts = new ObservableCollection<Account>();
        }

        /// <summary>
        /// Saves this sessions settings to file. 
        /// </summary>
        public void Save()
        {
            // Serialize the accounts. 
            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            var xw = XmlWriter.Create(SETTINGS_FILE);
            xs.Serialize(xw, this);
            xw.Dispose();
        }

        /// <summary>
        /// Load settings from last session from file. 
        /// </summary>
        public void Load()
        {
            if (!File.Exists(SETTINGS_FILE)) return;

            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            var xr = XmlReader.Create(SETTINGS_FILE);
            var Settings = (Settings)xs.Deserialize(xr);

            // Add loaded file's fields to this object. 
            this.Accounts = Settings.Accounts;
            this.InitialDirectory = Settings.InitialDirectory;
            this.ConfigurationPath = Settings.ConfigurationPath;
            this.ClientPath = Settings.ClientPath;
            this.ServerIP = Settings.ServerIP;

            xr.Dispose();
        }
    }
}
