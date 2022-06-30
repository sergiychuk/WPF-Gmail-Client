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

        private string username = "dspeakers8@gmail.com";
        private string password = "xgafnlmijfgputgr";
        bool autoFill = true;  // Вимкни цей флаг що б відключити автозаповнення данних!!!!

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

        private void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            using (var client = new ImapClient(new ProtocolLogger("imap.log")))
            {
                // Підключення клієнта із заданим сервером і портом
                client.Connect(server, port, SecureSocketOptions.SslOnConnect);
                
                // Спробуємо автентифікуватися по введеним логіну і паролю
                try
                {
                    client.Authenticate(txtboxUsername.Text, txtboxPassword.Password);
                    ///////////////////////////// [ DEBUGGING ] /////////////////////////////
                    foreach (var item in client.GetFolders(client.PersonalNamespaces[0]))
                    {
                        stringBuilder.AppendLine(item.Name);
                    }
                    MessageBox.Show(stringBuilder.ToString(), "FOLDERS");
                    ///////////////////////////// [ DEBUGGING ] /////////////////////////////
                }
                catch (AuthenticationException ex)
                {
                    MessageBox.Show($"{ex.Message}", "Authentication error");
                }
                client.Disconnect(true);
            }
        }
    }
}
