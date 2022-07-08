using System;
using System.Configuration;
using System.Collections.Generic;
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
using System.Windows.Navigation;
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
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Витягуємо з конфігу порт і сервер
        string server = ConfigurationManager.AppSettings["gmail_server"]; // витягуєм з конфіга адресу сервера
        int port = int.Parse(ConfigurationManager.AppSettings["gmail_port"]); // витягуєм з конфігу порт для підключення

        // логін і пароль для логіна в сервіс Gmail(передається з логін вінка)
        private string username;
        private string password;


        public MainWindow(string _username, string _password)
        {
            InitializeComponent();

            // встановлення логіну і паролю отриманого з логін вікна
            username = _username;
            password = _password;

            // ОТримоэмо список папок і записуємо їх в лістбокс folders_listbox
            RetriveFolders();
        }

        public async void RetriveFolders()
        {
            using(var client = new ImapClient())
            {
                // Асинхронне підключення клієнта із заданим сервером і портом
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                try
                {
                    await client.AuthenticateAsync(username, password);
                    var all = StatusItems.Recent;
                    var folders = await client.GetFoldersAsync(client.PersonalNamespaces[0], all, true);
                    foreach (var folder in folders)
                    {
                        listbox_folders.Items.Add(folder.FullName);
                    }
                }
                // Ловимо помилку при невдалому підключенні
                catch (AuthenticationException ex)
                {
                    MessageBox.Show($"{ex.Message}", "Authentication error");
                }
                await client.DisconnectAsync(true);
            }
        }
        private async void RetriveMessages(object sender, MouseButtonEventArgs e)
        {
            using (var client = new ImapClient())
            {
                // Асинхронне підключення клієнта із заданим сервером і портом
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                try
                {
                    await client.AuthenticateAsync(username, password);
                    // Вибираємо папку з лістбокса
                    var folder = await client.GetFolderAsync(((ListBox)sender).SelectedItem.ToString());
                    // Відкриваємо обрану папку
                    await folder.OpenAsync(FolderAccess.ReadOnly);
                    // Колекція листів
                    List<EmailListData> emailist = new List<EmailListData>();
                    int i = 0;
                    foreach (var item in folder)
                    {
                        emailist.Add(new EmailListData { Id = item.MessageId, Subject = item.Subject });
                        if (i < 10) i++;
                        else break;
                    }
                    listbox_messages.ItemsSource = emailist.AsEnumerable();
                }
                // Ловимо помилку при невдалому підключенні
                catch (AuthenticationException ex)
                {
                    MessageBox.Show($"{ex.Message}", "Authentication error");
                }
                await client.DisconnectAsync(true);
            }
        }

        private async void OpenMessage(object sender, MouseButtonEventArgs e)
        {
            using (var client = new ImapClient())
            {
                // Асинхронне підключення клієнта із заданим сервером і портом
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                try
                {
                    await client.AuthenticateAsync(username, password);

                    var messageid = ((EmailListData)((ListBox)sender).SelectedItem).Id;

                    // Вибираємо папку з лістбокса
                    var folder = await client.GetFolderAsync(listbox_folders.SelectedItem.ToString());
                    // Відкриваємо обрану папку
                    await folder.OpenAsync(FolderAccess.ReadOnly);
                    var message = folder.First(m => m.MessageId == messageid);

                    string messagebodytxt = message.TextBody;
                    //if (message.HtmlBody != null)
                    //{
                    //    messagebodytxt = message.HtmlBody;
                    //}
                    //else
                    //{
                    //    messagebodytxt = message.TextBody;
                    //}
                    MessageBox.Show(messagebodytxt, "Message text");
                }
                // Ловимо помилку при невдалому підключенні
                catch (AuthenticationException ex)
                {
                    MessageBox.Show($"{ex.Message}", "Authentication error");
                }
                await client.DisconnectAsync(true);
            }
        }
    }

    public class EmailListData
    {
        public string Id { get; set; }
        public string Subject { get; set; }
    }
}
