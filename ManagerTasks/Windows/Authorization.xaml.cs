using ManagerTasks.Classes;
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

namespace ManagerTasks.Windows
{

    public partial class Authorization : Window
    {

        private Database _database;

        public Authorization()
        {
            InitializeComponent();
            _database = Database.GetInstance();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;


            if (username == string.Empty || password == string.Empty)
            {
                MessageBox.Show("Введены не все данные");
            }

            else
            {
                var user = _database.LoginUser(username, password);
                if (user != null)
                {

                    _database.SetUsernameForDB(username);
                    Tasks tasksWindow = new Tasks();
                    tasksWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Неверный логин или пароль");
                }
            }

            
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            Registration registerWindow = new Registration();
            registerWindow.Show();
            this.Close();
        }

        private void RecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            Recovery RecoveryWindow = new Recovery();
            RecoveryWindow.Show();
            this.Close();
        }
    }
}
