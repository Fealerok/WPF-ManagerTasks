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
            _database = Database.GetInstance();
            _team = team;


            DataContext = _team;


            LoadUsers();
        }


        private void LoadUsers()
        {
            var allUsers = _database.GetUsers();
            var teamUserIds = _team.Users.Select(u => u.Id).ToList();
            UsersComboBox.ItemsSource = allUsers.Where(u => !teamUserIds.Contains(u.Id)).ToList();
        }


        private void AddUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersComboBox.SelectedItem as User;
            if (selectedUser != null)
            {

                _team.Users.Add(selectedUser);
                _database.AddUserToTeam(_team.Id, selectedUser.Id);


                LoadUsers();
                TeamMembersListBox.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Please select a user to add.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            _database.UpdateTeam(_team);
            DialogResult = true; 
            Close();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; 
            Close();
        }
    }
}
