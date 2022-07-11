using System;
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

namespace GmailClient
{
    /// <summary>
    /// Логика взаимодействия для OpenMessageWindow.xaml
    /// </summary>
    public partial class OpenMessageWindow : Window
    {
        //private string sender;
        //private string theme;
        //private string message;

        public OpenMessageWindow(string _sender, string _theme, string _message)
        {
            InitializeComponent();
            txtboxFromWhom.Text = _sender;
            txtboxTheme.Text = _theme;
            txtboxBody.Text = _message;
        }

        private void button_Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
