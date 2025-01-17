using ManagerTasks.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ManagerTasks.Windows
{
    public partial class PersonalAccountWindow : Window
    {
        private Database _database;
        private User _currentUser;
        private List<Classes.Task> _userTasks;

        public PersonalAccountWindow()
        {
            InitializeComponent();
            _database = Database.GetInstance();

            // Получаем текущего пользователя
            _currentUser = _database.GetCurrentUser();

            // Отображаем информацию о пользователе
            UsernameLabel.Content = _currentUser.Username;
            EmailLabel.Content = _currentUser.Email;

            // Загружаем задачи, назначенные на текущего пользователя
            LoadUserTasks();
        }

        private void LoadUserTasks()
        {
            // Получаем все задачи из базы данных
            var allTasks = _database.GetTasks();


            // Фильтруем задачи, назначенные на текущего пользователя
            _userTasks = allTasks.Where(t => t.AssignedUserId == _currentUser.Id).ToList();

            // Отображаем задачи в DataGrid
            TasksGrid.ItemsSource = _userTasks;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Recovery recoveryWindow = new Recovery();
            recoveryWindow.Show();
            this.Close();
        }
    }
}
