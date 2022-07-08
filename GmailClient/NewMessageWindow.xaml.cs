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
    /// Логика взаимодействия для NewMessageWindow.xaml
    /// </summary>
    public partial class NewMessageWindow : Window
    {
        // Витягуємо з конфігу порт і сервер
        string server = ConfigurationManager.AppSettings["smtp_server"]; // витягуєм з конфіга адресу сервера
        int port = int.Parse(ConfigurationManager.AppSettings["smtp_port"]); // витягуєм з конфігу порт для підключення
        
        // логін і пароль для логіна в сервіс Gmail(передається з MainWindow)
        private string username;
        private string password;

        public NewMessageWindow(string _username, string _password)
        {
            InitializeComponent();

            // встановлення логіну і паролю отриманого з MainWindow
            username = _username;
            password = _password;
        }

        private void button_Send_Click(object sender, RoutedEventArgs e)
        {
            // створюємо об'єкт повідомлення
            MimeMessage message = new MimeMessage();
            // заповнюємо дані для відправки "від кого"
            message.From.Add(new MailboxAddress(txtboxFromWhom.Text, username));
            // заповнюємо дані для відправки "кому"
            message.To.Add(MailboxAddress.Parse(txtboxToWhom.Text));
            // заповнюємо тему повідомлення, якщо тема не задана то ставимо "No theme"
            message.Subject = txtboxTheme.Text == "" ? "No theme" : txtboxTheme.Text;
            // заповнюємо тіло повідомлення
            message.Body = new TextPart("plain")
            {
                Text = txtboxBody.Text
            };
            // встановлюємо приорітет повідомлення
            message.Importance = MessageImportance.High;
            // відправка повідомлення через SmtpClient
            SmtpClient client = new SmtpClient();
            try
            {
                // підключення по заданому порту і серверу які вказані в конфіг файлі
                client.Connect(server, port, true);
                // аутентифікація по отриманим данним з MainWindow
                client.Authenticate(username, password);
                // відправка повідомлення
                client.Send(message);
                // звітуємо що повідомлення було відправлене
                MessageBox.Show("Email was sent!");
            }
            catch (Exception ex)
            {
                // показуємо помилку якщо така трапилася
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // відключення smtp клієнта
                client.Disconnect(true);
                client.Dispose();
                // закриваємо вікно відправки повідомлення
                this.Close();
            }
        }
    }
}
