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
    /// <summary>
    /// Логика взаимодействия для Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        private Database _database;

        public Registration()
        {
            InitializeComponent();
            _database = Database.GetInstance();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            var user = new User
            {
                FullName = fullName,
                Email = email,
                Username = username,
                Password = password
            };

            if (user.FullName == string.Empty ||
                    user.Email == string.Empty ||
                    user.Username == string.Empty ||
                    user.Password == string.Empty)
            {
                MessageBox.Show("Неудачная регистрация.");
            }

            else
            {
                if (_database.RegisterUser(user))
                {
                    MessageBox.Show("Успешная регистрация.");
                    Authorization loginWindow = new Authorization();
                    loginWindow.Show();
                    this.Close();
                }

                else
                {
                    MessageBox.Show("Неудачная регистрация.");
                }
            }

        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Authorization loginWindow = new Authorization();
            loginWindow.Show();
            this.Close();
        }
    }
}
