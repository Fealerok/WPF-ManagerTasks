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
    /// Логика взаимодействия для Users.xaml
    /// </summary>
    public partial class Users : Window
    {
        private Database _database;

        public Users()
        {
            InitializeComponent();
            _database = Database.GetInstance(); // Инициализация базы данных
            LoadUsers(); // Загрузка пользователей при открытии окна
        }

        // Загрузка пользователей из базы данных
        private void LoadUsers()
        {
            UsersGrid.ItemsSource = _database.GetUsersWithDetails();
        }


        // Обработчик кнопки "Edit User"
        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                // Открываем окно редактирования пользователя
                var editUserWindow = new EditUserWindow(selectedUser);
                if (editUserWindow.ShowDialog() == true)
                {
                    // Если пользователь нажал "Save", обновляем список пользователей
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик кнопки "Delete User"
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                // Удаляем пользователя из базы данных
                _database.DeleteUser(selectedUser.Id);
                LoadUsers(); // Обновляем список пользователей
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик кнопки "Back to Tasks"
        private void BackToTasksButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно Tasks
            var tasksWindow = new Tasks();
            tasksWindow.Show(); // Показываем окно Tasks
            this.Close(); // Закрываем текущее окно Users
        }
    }
}
