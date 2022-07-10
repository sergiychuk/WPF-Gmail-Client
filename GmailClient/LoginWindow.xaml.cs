using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using MailKit.Search;
using MailKit.Security;
using MimeKit;

namespace GmailClient
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string server = ConfigurationManager.AppSettings["gmail_server"]; // sets the server address
        int port = int.Parse(ConfigurationManager.AppSettings["gmail_port"]); //sets the server port

        private readonly string username = "asdasdasdasdasd@gmail.com"; // paste here your email
        private readonly string password = "asdasdasdasdasdasd"; // paste here your password 
        private bool autoFill = false;  // Turning off this flag will turn off autofill credentials

        public LoginWindow()
        {
            InitializeComponent();
            if (autoFill) AutoSetCredentials();
        }

        public void AutoSetCredentials()
        {
            txtboxUsername.Text = username;
            txtboxPassword.Password = password;
        }

        private async void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new ImapClient())
            {
                // Asynchronous connection of the client with the specified server and port
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);

                // Try to asynchronously authenticate using the login and password entered by the user
                try
                {
                    await client.AuthenticateAsync(txtboxUsername.Text, txtboxPassword.Password);

                    // If the authentication is successful, transfer the login data to the MainWindow window and start it
                    if (client.IsAuthenticated)
                    {
                        // Create MainWindow object
                        MainWindow mainWindow = new MainWindow(txtboxUsername.Text, txtboxPassword.Password);
                        // Open Mainwindow with given credentials for login
                        mainWindow.Show();
                        // Disconnecting client
                        await client.DisconnectAsync(true);
                        // Close LoginWindow
                        this.Close();
                    }
                    #region [ DEBUG ]
                    //var all = StatusItems.Recent;
                    //var folders = await client.GetFoldersAsync(client.PersonalNamespaces[0], all, true);
                    //foreach (var folder in folders)
                    //{
                    //    MessageBox.Show($"{folder.FullName}");
                    //}
                    #endregion
                }
                catch (AuthenticationException ex)
                {
                    MessageBox.Show($"{ex.Message}", "Authentication error");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
