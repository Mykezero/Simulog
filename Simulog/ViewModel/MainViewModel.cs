using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using Simulog.Model;
using Simulog.ViewModel;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;

namespace Simulog
{
    /// <summary>
    /// Launches multiple sessions based on the configuration file
    /// and account information set by the user. 
    /// </summary>
    public class MainViewModel : IViewModel
    {
        /// <summary>
        /// Client used for launching sessions. 
        /// </summary>
        private static readonly string PROGRAM_NAME = "Ashita";
        
        /// <summary>
        /// Client used for launching sessions. Includes the extension. 
        /// </summary>
        private static readonly string PROGRAM_NAME_LONG = string.Empty;

        /// <summary>
        /// The client's extension (.exe)
        /// </summary>
        private static readonly string PROGRAM_EXTENSION = ".exe";
        
        /// <summary>
        /// The command line argument that allows us to launch the client
        /// by specifying a configuration file to use. 
        /// </summary>
        private static readonly string COMMAND_ARGUMENT = "--config=";

        public ISettings Settings { get; set; }

        static MainViewModel()
        {
            PROGRAM_NAME_LONG = PROGRAM_NAME + PROGRAM_EXTENSION;
        }

        /// <summary>
        /// Set settings and create command bindings. 
        /// </summary>
        /// <param name="settings"></param>
        public MainViewModel(ISettings settings)
        {
            this.Settings = settings;
            this.LoginCommand = new DelegateCommand(Login);
            this.SetClientCommand = new DelegateCommand(SetClient);
            this.SetConfigurationCommand = new DelegateCommand(SetConfiguration);
        }

        /// <summary>
        /// UI binds to this for setting the configuration file. 
        /// </summary>
        public ICommand SetConfigurationCommand { get; set; }

        /// <summary>
        /// Ask user to select the path to a 
        /// configuration file. 
        /// </summary>
        private void SetConfiguration()
        {
            var path = PromptUserForFile();
            if (string.IsNullOrWhiteSpace(path)) return;
            Settings.ConfigurationPath = path;
        }

        /// <summary>
        /// UI binds to this for setting the client. 
        /// </summary>
        public ICommand SetClientCommand { get; set; }

        /// <summary>
        /// Ask user to select the path to their 
        /// Ashita client. 
        /// </summary>
        private void SetClient()
        {
            var path = PromptUserForFile();
            if (string.IsNullOrWhiteSpace(path)) return;            
            Settings.ClientPath = path;
        }

        /// <summary>
        /// Prompts user to select a file and returns the 
        /// path to that file. 
        /// </summary>
        /// <returns></returns>
        private String PromptUserForFile()
        {
            // Prompt user to select a file. 
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Settings.InitialDirectory;
            ofd.ShowDialog();

            // Set file if one was found. 
            if (File.Exists(ofd.FileName))
            {
                Settings.InitialDirectory = ofd.FileName;
                return ofd.FileName;
            }
            else return string.Empty;
        }

        /// <summary>
        /// Login all accounts
        /// </summary>
        public ICommand LoginCommand { get; set; }

        /// <summary>
        /// Log all added accounts into the server. 
        /// </summary>
        private void Login()
        {
            // Inform user that their configuration 
            // file could not be found. Maybe they forgot to
            // set it. 
            if (!File.Exists(Settings.ConfigurationPath))
            {
                var message = DocString(@"The configuration file was not found, 
                and must be set before logging in.");
                var caption = "Configuration file could not found";
                MessageBox.Show(message, caption,
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Inform user that this program is not in the 
            // same directory as the target program.
            if (!File.Exists(Settings.ClientPath))
            {
                var message = DocString(
                    @"{0} could not be found. Please make sure you've 
                    set the path to {1}.",
                    PROGRAM_NAME, PROGRAM_NAME_LONG);
                var caption = DocString("{0} not found", PROGRAM_NAME);
                MessageBox.Show(message, caption,
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Change our directory into the Ashita's directory. 
            Directory.SetCurrentDirectory(Directory.GetParent
                (Settings.ClientPath).FullName);

            // Log each account onto the private server of choice. 
            foreach (var account in Settings.Accounts)
            {
                // Get the path to the about to be created configuration file. 
                var copiedConfigurationPath = Path.Combine(Directory.GetParent(
                    Settings.ConfigurationPath).FullName, account.Name);

                // Create a new file from the user's selected configuration file. 
                File.Copy(Settings.ConfigurationPath, copiedConfigurationPath, true);

                // Alter the new configuration file with the account data. 
                AlterConfigurationXml(copiedConfigurationPath, account);

                // Start a new client session for the account 
                Process.Start(Settings.ClientPath, COMMAND_ARGUMENT + copiedConfigurationPath);
            }
        }

        /// <summary>
        /// Remove all newline characters and multiple spaces to allow
        /// easy wrapping of long lines with verbatim strings. 
        /// <example>
        ///     String message = DocString(@"A very, very, very, very, 
        ///     very, very long string!");
        /// </example>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private String DocString(string message, params string[] values)
        {
            return Regex.Replace(String.Format(message, values)
                .Replace(Environment.NewLine, ""), @"\s+", " ");
        }

        /// <summary>
        /// Alters the xml configuration file to match the 
        /// given account.
        /// </summary>
        /// <param name="account"></param>
        private void AlterConfigurationXml(String path, Account account)
        {
            // Find the current attribute that contains 
            // the boot command. 
            XDocument copy = XDocument.Load(path);
            var command = copy.Elements("settings").Elements("setting")
                .Where(x => x.Attribute("name")
                .Value == "boot_command")
                .FirstOrDefault();
            if (command == null) return;

            // Alter the xml's boot command attribute for 
            // the new account. 
            command.Value = CreateBootCommand(
                Settings.ServerIP,
                account.Name,
                account.Password);

            // Write the modified xml back to file. 
            copy.Save(path);
        }

        /// <summary>
        /// Returns the proper format for Ashita's boot command
        /// attribute given a server ip, account name and 
        /// account password. 
        /// </summary>
        /// <param name="server"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private string CreateBootCommand(String server, String user, String password)
        {
            return "--server " + server + " --user " + user + " --pass " + password;
        }
    }
}
