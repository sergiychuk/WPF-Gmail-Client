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

        private readonly string username = "dspeakers8@gmail.com";
        private readonly string password = "xgafnlmijfgputgr";
        private bool autoFill = true;  // Вимкни цей флаг що б відключити автозаповнення данних!!!!

        public LoginWindow()
        {
            InitializeComponent();

            if (autoFill)
            {
                AutoSetCredentials();
            }
        }

        public void AutoSetCredentials()
        {
            txtboxUsername.Text = username;
            txtboxPassword.Password = password;
        }

        private async void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            //using (var client = new ImapClient(new ProtocolLogger("imap.log")))
            using (var client = new ImapClient())
            {
                // Асинхронне підключення клієнта із заданим сервером і портом
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                
                // Спробуємо асинхронно автентифікуватися по введеним логіну і паролю
                try
                {
                    //client.Authenticate(txtboxUsername.Text, txtboxPassword.Password);
                    await client.AuthenticateAsync(txtboxUsername.Text, txtboxPassword.Password);

                    // Якщо успішна автентифікація то передаємо дані для логіну у вікно MainWindow і запускаскаємо його закривши це вікно
                    if (client.IsAuthenticated)
                    {
                        MainWindow mainWindow = new MainWindow(txtboxUsername.Text, txtboxPassword.Password);
                        // Відключення клієнта
                        await client.DisconnectAsync(true);
                        mainWindow.Show();
                        this.Close();
                    }

                    ///////////////////////////// [ DEBUGGING ] /////////////////////////////
                    //MessageBox.Show($"{client.IsAuthenticated}");
                    //StringBuilder stringBuilder = new StringBuilder();

                    //var folders = await client.GetFoldersAsync(client.PersonalNamespaces[0], all, true);
                    //foreach (var folder in folders)
                    //{
                    //    MessageBox.Show($"{folder.FullName}");
                    //}
                    
                    //var all = StatusItems.Recent;

                    //foreach (var item in client.GetFolders(client.PersonalNamespaces[0]))
                    //foreach (var item in await client.GetFoldersAsync(client.PersonalNamespaces[0], all, true))
                    //{
                    //    stringBuilder.AppendLine(item.Name);
                    //}
                    //MessageBox.Show(stringBuilder.ToString(), "FOLDERS");
                    ///////////////////////////// [ DEBUGGING ] /////////////////////////////
                }
                // Ловимо помилку при невдалому підключенні
                catch (AuthenticationException ex)
                {
                    MessageBox.Show($"{ex.Message}", "Authentication error");
                }

                // Відключення клієнта
                //await client.DisconnectAsync(true);
            }
        }
    }
}
