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
    /// Логика взаимодействия для Recovery.xaml
    /// </summary>
    public partial class Recovery : Window
    {
        private Database _database;
        public Recovery()
        {
            InitializeComponent();
            _database = Database.GetInstance();
        }

        private void RecoveryButton_Click(object sender, RoutedEventArgs e)
        {
            string login = UsernameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = PasswordBox.Password.ToString();

            if (login == string.Empty || email == string.Empty)
            {
                MessageBox.Show("Логин или почта не введены");
            }

            else
            {
                if (_database.CheckUserData(login, email))
                {
                    if (password != string.Empty) 
                    {
                        _database.SetNewPassword(password, login);
                        MessageBox.Show("Пароль успешно восстановлен");
                        Authorization loginWindow = new Authorization();
                        loginWindow.Show();
                        this.Close();
                    }

                    else MessageBox.Show("Введите новый пароль");
                }

                else MessageBox.Show("Неверные логнин или пароль");
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
