using ManagerTasks.Classes;
using System.Windows;

namespace ManagerTasks.Windows
{
    public partial class Teams : Window
    {
        private Database _database;

        public Teams()
        {
            InitializeComponent();
            _database = new Database(); // Инициализация базы данных
            LoadTeams(); // Загрузка команд при открытии окна
        }

        // Загрузка команд из базы данных
        private void LoadTeams()
        {
            TeamsGrid.ItemsSource = _database.GetTeams();
        }

        // Обработчик кнопки "Add Team"
        private void AddTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var team = new Team
            {
                Name = "New Team"
            };

            _database.AddTeam(team);
            LoadTeams(); // Обновляем список команд
        }

        // Обработчик кнопки "Edit Team"
        private void EditTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTeam = TeamsGrid.SelectedItem as Team;
            if (selectedTeam != null)
            {
                // Открываем окно редактирования команды
                var editTeamWindow = new EditTeamWindow(selectedTeam);
                if (editTeamWindow.ShowDialog() == true)
                {
                    // Если пользователь нажал "Save", обновляем список команд
                    LoadTeams();
                }
            }
            else
            {
                MessageBox.Show("Please select a team to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик кнопки "Delete Team"
        private void DeleteTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTeam = TeamsGrid.SelectedItem as Team;
            if (selectedTeam != null)
            {
                // Удаляем команду из базы данных
                _database.DeleteTeam(selectedTeam.Id);
                LoadTeams(); // Обновляем список команд
            }
            else
            {
                MessageBox.Show("Please select a team to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик кнопки "Back to Tasks"
        private void BackToTasksButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно Tasks
            var tasksWindow = new Tasks();
            tasksWindow.Show(); // Показываем окно Tasks
            this.Close(); // Закрываем текущее окно Teams
        }
    }
}
