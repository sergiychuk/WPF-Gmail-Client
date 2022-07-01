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

            ///////////////////////////// [ DEBUGGING ] /////////////////////////////
            //this.Title = $"Username: {username}, Password: {password}";
        }

        public void FetchData()
        {

        }
    }
}
