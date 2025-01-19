using ManagerTasks.Classes;
using System.Windows;

namespace ManagerTasks.Windows
{
    public partial class EditUserWindow : Window
    {
        private Database _database;
        private User _user;

        public EditUserWindow(User user)
        {
            InitializeComponent();
            _database = Database.GetInstance(); 
            _user = user;


            DataContext = _user;


            RoleComboBox.ItemsSource = _database.GetRoles();
            TeamComboBox.ItemsSource = _database.GetTeams();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            _database.UpdateUser(_user);
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