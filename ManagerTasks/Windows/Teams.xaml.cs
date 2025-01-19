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
            _database = Database.GetInstance();
            LoadTeams(); 
        }

        
        private void LoadTeams()
        {
            TeamsGrid.ItemsSource = _database.GetTeams();
        }

        
        private void AddTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var team = new Team
            {
                Name = "New Team"
            };

            _database.AddTeam(team);
            LoadTeams(); 
        }

        
        private void EditTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTeam = TeamsGrid.SelectedItem as Team;
            if (selectedTeam != null)
            {
               
                var editTeamWindow = new EditTeamWindow(selectedTeam);
                if (editTeamWindow.ShowDialog() == true)
                {
                    
                    LoadTeams();
                }
            }
            else
            {
                MessageBox.Show("Please select a team to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void DeleteTeamButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTeam = TeamsGrid.SelectedItem as Team;
            if (selectedTeam != null)
            {
                
                _database.DeleteTeam(selectedTeam.Id);
                LoadTeams(); 
            }
            else
            {
                MessageBox.Show("Please select a team to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

  
        private void BackToTasksButton_Click(object sender, RoutedEventArgs e)
        {

            var tasksWindow = new Tasks();
            tasksWindow.Show(); 
            this.Close(); 
        }
    }
}
