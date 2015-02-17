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
        private static readonly string SETTINGS_FILE = "settings.xml";

        private string _serverIP;
        public string ServerIP
        {
            get { return _serverIP; }
            set { SetProperty(ref _serverIP, value); }
        }

        private string initialDirectory;

        public string InitialDirectory
        {
            get { return initialDirectory; }
            set { initialDirectory = value; }
        }


        private string _configurationPath;
        public string ConfigurationPath
        {
            get { return _configurationPath; }
            set { SetProperty(ref _configurationPath, value); }
        }

        private string _clientPath;
        public string ClientPath
        {
            get { return _clientPath; }
            set { SetProperty(ref _clientPath, value); }
        }

        public ObservableCollection<Account> Accounts { get; set; }

        public Settings()
        {
            this.ServerIP = "127.0.0.1";
            this.InitialDirectory = System.Environment.CurrentDirectory;
            this.ConfigurationPath = string.Empty;
            this.Accounts = new ObservableCollection<Account>();
        }

        public void Save()
        {
            // Serialize the accounts. 
            XmlSerializer xs = new XmlSerializer(typeof(Settings));
            var xw = XmlWriter.Create(SETTINGS_FILE);
            xs.Serialize(xw, this);
            xw.Dispose();
        }

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
