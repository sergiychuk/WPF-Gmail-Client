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
        // port and server retrivering from App.config
        string server = ConfigurationManager.AppSettings["gmail_server"]; // витягуєм з конфіга адресу сервера
        int port = int.Parse(ConfigurationManager.AppSettings["gmail_port"]); // витягуєм з конфігу порт для підключення

        // credentials for login in gmail services (retrivering from LoginWindow)
        private string username;
        private string password;


        public MainWindow(string _username, string _password)
        {
            InitializeComponent();

            // set login credentials
            username = _username;
            password = _password;

            // retriving mail folders and store them into folders_listbox
            //RetriveFolders();
            RetriveAllMessages();
            //GetMeassges();
        }

        //public async void RetriveFolders()
        //{
        //    using(var client = new ImapClient())
        //    {
        //        // Асинхронне підключення клієнта із заданим сервером і портом
        //        await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
        //        try
        //        {
        //            await client.AuthenticateAsync(username, password);
        //            var all = StatusItems.Recent;
        //            var folders = await client.GetFoldersAsync(client.PersonalNamespaces[0], all, true);
        //            foreach (var folder in folders)
        //            {
        //                //listbox_folders.Items.Add(folder.FullName);
        //            }
        //        }
        //        // Ловимо помилку при невдалому підключенні
        //        catch (AuthenticationException ex)
        //        {
        //            MessageBox.Show($"{ex.Message}", "Authentication error");
        //        }
        //        await client.DisconnectAsync(true);
        //    }
        //}


        #region [ Retrivering email messages ]
        private async void RetriveAllMessages()
        {
            using (var client = new ImapClient())
            {
                // Connecting by given port and server
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                // Try authenticate and do some magic
                try
                {
                    await client.AuthenticateAsync(username, password);
                    // Get folder with all messages(INBOX all messages)
                    var folder = client.GetFolder(SpecialFolder.All);
                    // Open selected folder
                    await folder.OpenAsync(FolderAccess.ReadOnly);
                    // Get messages ids in array
                    var uids = folder.Search(SearchQuery.All);
                    // Get messages total count
                    int messagesCount = uids.Count - 1;
                    // Amount of messages that will be downloaded (this counter for pagination)
                    int messagesPerPage = 2;
                    // Create collection for storing messages
                    List<EmailListData> emailist = new List<EmailListData>();
                    // Download given amount of messages and store them to collection
                    for (int i = messagesCount; i > messagesCount - messagesPerPage; i--)
                    {
                        emailist.Add(new EmailListData { Id = i, Subject = folder.GetMessage(i).Subject });
                    }
                    // Set items source for listbox
                    listbox_inbox_messages.ItemsSource = emailist.AsEnumerable();
                }
                // Catch errors if got exceptions
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

        private async void GetMeassges()
        {
            using (var client = new ImapClient())
            {
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                try
                {
                    await client.AuthenticateAsync(username, password);
                    var folder = await client.GetFolderAsync("INBOX");
                    // Open selected folder
                    await folder.OpenAsync(FolderAccess.ReadOnly);

                    //var uids = client.Inbox.Search(SearchQuery.All);
                    //var messages = uids.Select(x => client.Inbox.GetMessage(x));
                    //var sortedMessages = messages.OrderByDescending(x => x.Date);

                    var lastMessages = Enumerable.Range(client.Inbox.Count - 3, 3).ToList();
                    var messages = client.Inbox.Fetch(lastMessages, MessageSummaryItems.UniqueId);
                    foreach (var message in messages)
                    {
                        MessageBox.Show(message.TextBody.ToString(), "Message");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "ERROR");
                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
        #endregion

        //private async void OpenMessage(object sender, MouseButtonEventArgs e)
        //{
        //    using (var client = new ImapClient())
        //    {
        //        // Асинхронне підключення клієнта із заданим сервером і портом
        //        await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
        //        try
        //        {
        //            await client.AuthenticateAsync(username, password);

        //            var messageid = ((EmailListData)((ListBox)sender).SelectedItem).Id;

        //            // Вибираємо папку з лістбокса
        //            //var folder = await client.GetFolderAsync(listbox_folders.SelectedItem.ToString());

        //            // Відкриваємо обрану папку
        //            //await folder.OpenAsync(FolderAccess.ReadOnly);
        //            //var message = folder.First(m => m.MessageId == messageid);

        //            //string messagebodytxt = message.TextBody;
        //            //if (message.HtmlBody != null)
        //            //{
        //            //    messagebodytxt = message.HtmlBody;
        //            //}
        //            //else
        //            //{
        //            //    messagebodytxt = message.TextBody;
        //            //}
        //            //MessageBox.Show(messagebodytxt, "Message text");
        //        }
        //        // Ловимо помилку при невдалому підключенні
        //        catch (AuthenticationException ex)
        //        {
        //            MessageBox.Show($"{ex.Message}", "Authentication error");
        //        }
        //        await client.DisconnectAsync(true);
        //    }
        //}

        #region [ Menu items handlers ]
        private void menuitem_new_message_Click(object sender, RoutedEventArgs e)
        {
            NewMessageWindow newMessageWindow = new NewMessageWindow(username, password);
            newMessageWindow.Show();
        }
        private void menuitem_logout_Click(object sender, RoutedEventArgs e)
        {
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
        private void menuitem_exit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void menuitem_about_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ABOUT INFO HERE!");
        }
        #endregion
    }

    /// <summary>
    /// Class for storing email lists data
    /// </summary>
    public class EmailListData
    {
        public int Id { get; set; }
        public string Subject { get; set; }
    }
}
