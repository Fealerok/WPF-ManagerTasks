using ManagerTasks.Classes;
using System.Linq;
using System.Windows;

namespace ManagerTasks.Windows
{
    public partial class EditTeamWindow : Window
    {
        private Database _database;
        private Team _team;

        public EditTeamWindow(Team team)
        {
            InitializeComponent();
            _database = new Database(); // Инициализация базы данных
            _team = team;

            // Привязка данных команды к элементам управления
            DataContext = _team;

            // Загрузка пользователей для ComboBox
            LoadUsers();
        }

        // Загрузка пользователей для ComboBox
        private void LoadUsers()
        {
            var allUsers = _database.GetUsers();
            var teamUserIds = _team.Users.Select(u => u.Id).ToList();
            UsersComboBox.ItemsSource = allUsers.Where(u => !teamUserIds.Contains(u.Id)).ToList();
        }

        // Обработчик кнопки "Add User"
        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersComboBox.SelectedItem as User;
            if (selectedUser != null)
            {
                // Добавляем пользователя в команду
                _team.Users.Add(selectedUser);
                _database.AddUserToTeam(_team.Id, selectedUser.Id);

                // Обновляем ComboBox и ListBox
                LoadUsers();
                TeamMembersListBox.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Please select a user to add.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик кнопки "Save"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение изменений в базе данных
            _database.UpdateTeam(_team);
            DialogResult = true; // Закрыть окно с результатом "ОК"
            Close();
        }

        // Обработчик кнопки "Cancel"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрыть окно с результатом "Отмена"
            Close();
        }
    }
}
