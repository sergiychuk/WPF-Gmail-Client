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

        // Amount of messages that will be downloaded (this counter for pagination)
        public int messagesPerPage = 5;

        public MainWindow(string _username, string _password)
        {
            InitializeComponent();

            // set login credentials
            username = _username;
            password = _password;

            // Retriving all messages 
            RetriveMessages();
            textbox_messages_per_page.Text = messagesPerPage.ToString();
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
        private void menuitem_refresh_Click(object sender, RoutedEventArgs e)
        {
            RetriveMessages();
        }
        #endregion

        #region [ Retrivering messages ]
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
                    // Get folder with all messages
                    var folder = client.GetFolder(SpecialFolder.All);
                    // Open selected folder
                    await folder.OpenAsync(FolderAccess.ReadOnly);
                    // Get messages ids in array
                    var uids = folder.Search(SearchQuery.All);
                    // Get messages total count
                    int messagesCount = uids.Count - 1;
                    // Create collection for storing messages
                    List<EmailListData> emailist = new List<EmailListData>();
                    // Download given amount of messages and store them to collection mails
                    for (int i = messagesCount; i > messagesCount - messagesPerPage; i--)
                    {
                        emailist.Add(new EmailListData { Id = i, Subject = folder.GetMessage(i).Subject });
                    }
                    // Set items source for listbox
                    listbox_all_messages.ItemsSource = emailist.AsEnumerable();
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
        private async void RetriveSendedMessages()
        {
            using (var client = new ImapClient())
            {
                // Connecting by given port and server
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);
                // Try authenticate and do some magic
                try
                {
                    await client.AuthenticateAsync(username, password);
                    // Get folder with all sended messages
                    var folder = client.GetFolder(SpecialFolder.Sent);
                    // Open selected folder
                    await folder.OpenAsync(FolderAccess.ReadOnly);
                    // Get messages ids in array
                    var uids = folder.Search(SearchQuery.All);
                    // Get messages total count
                    int messagesCount = uids.Count - 1;
                    // Create collection for storing messages
                    List<EmailListData> emailist = new List<EmailListData>();
                    // Download given amount of messages and store them to collection mails
                    for (int i = messagesCount; i > messagesCount - messagesPerPage; i--)
                    {
                        emailist.Add(new EmailListData { Id = i, Subject = folder.GetMessage(i).Subject });
                    }
                    // Set items source for listbox
                    listbox_sended_messages.ItemsSource = emailist.AsEnumerable();
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
        #endregion

        #region [ Updating data ]
        private void RetriveMessages()
        {
            //statusbaritem_update_status.Content = "Updating...";
            RetriveAllMessages();
            RetriveSendedMessages();
            textbox_last_update_time.Text = $"{DateTime.Now}";
            //statusbaritem_update_status.Content = $"Updated: {DateTime.Now}";
        }
        #endregion

        private void textbox_messages_per_page_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(textbox_messages_per_page.Text != null && textbox_messages_per_page.Text != "")
            {
                var isNumeric = int.TryParse(textbox_messages_per_page.Text, out int n);
                if (isNumeric)
                {
                    messagesPerPage = n;
                    RetriveMessages();
                }
                else
                {
                    MessageBox.Show("Only numbers!");
                    textbox_messages_per_page.Text = messagesPerPage.ToString();
                }
            }
            if(textbox_messages_per_page.Text != "")
            {
                textbox_messages_per_page.Text = messagesPerPage.ToString();
            }
        }
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
