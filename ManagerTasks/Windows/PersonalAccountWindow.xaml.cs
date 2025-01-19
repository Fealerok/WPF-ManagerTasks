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


            _currentUser = _database.GetCurrentUser();


            UsernameLabel.Text = _currentUser.Username;
            EmailLabel.Text = _currentUser.Email;


            LoadUserTasks();
        }

        private void LoadUserTasks()
        {

            var allTasks = _database.GetTasks();


            _userTasks = allTasks.Where(t => t.AssignedUserId == _currentUser.Id).ToList();


            TasksGrid.ItemsSource = _userTasks;
        }


        private void RecoveryButton_Click(object sender, RoutedEventArgs e)
        {

            Recovery recoveryWindow = new Recovery();
            recoveryWindow.Show();
            this.Close();
        }

 
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {

            Tasks tasksWindow = new Tasks();
            tasksWindow.Show();
            this.Close(); 
        }
    }
}
