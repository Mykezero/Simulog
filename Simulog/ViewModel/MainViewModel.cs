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
        private static readonly string PROGRAM_NAME = "Ashita";
        private static readonly string PROGRAM_NAME_LONG = string.Empty;
        private static readonly string PROGRAM_EXTENSION = ".exe";
        private static readonly string COMMAND_ARGUMENT = "--config=";

        public ISettings Settings { get; set; }

        static MainViewModel()
        {
            PROGRAM_NAME_LONG = PROGRAM_NAME + PROGRAM_EXTENSION;
        }

        public MainViewModel(ISettings settings)
        {
            this.Settings = settings;
            this.LoginCommand = new DelegateCommand(Login);
            this.SetCommand = new DelegateCommand(Set);
        }

        public ICommand SetCommand { get; set; }

        private void Set()
        {
            // Prompt user to select configuration file. 
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = Environment.CurrentDirectory;
            ofd.ShowDialog();
            
            // Set file if one was found. 
            if (ofd.CheckFileExists)
            {
                Settings.ConfigurationFilePath = ofd.FileName;
            }
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
            if (!File.Exists(Settings.ConfigurationFilePath))
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
            if (!File.Exists(PROGRAM_NAME_LONG))
            {
                var message = DocString(
                    @"{0} could not be found. Please make sure this 
                    executable is in the same folder as {1}.",
                    PROGRAM_NAME, PROGRAM_NAME_LONG);
                var caption = DocString("{0} not found", PROGRAM_NAME);
                MessageBox.Show(message, caption,
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Log each account onto the private server of choice. 
            foreach (var account in Settings.Accounts)
            {
                // Create a new file from the user's selected configuration file. 
                File.Copy(Settings.ConfigurationFilePath, account.Name, true);
                AlterConfigurationXml(account);
                File.Copy(account.Name, FindPath(account.Name), true);
                Process.Start(PROGRAM_NAME, COMMAND_ARGUMENT + account.Name);
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
        private void AlterConfigurationXml(Account account)
        {
            // Find the current attribute that contains 
            // the boot command. 
            XDocument copy = XDocument.Load(account.Name);
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
            copy.Save(account.Name);
        }

        private String FindPath(string filename)
        {
            var parent = new FileInfo(Settings.ConfigurationFilePath).Directory.FullName;
            return Path.Combine(parent, filename);
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
