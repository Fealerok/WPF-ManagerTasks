using ManagerTasks.Classes;
using System;
using System.Windows;

namespace ManagerTasks.Windows
{
    public partial class EdingTask : Window
    {
        private Database _database;
        private Classes.Task _task;

        public EdingTask(Classes.Task task)
        {
            InitializeComponent();
            _database = Database.GetInstance(); 
            _task = task;


            DataContext = _task;


            LoadStatuses();
            LoadUsers();
            LoadProjects();
        }

        private void LoadStatuses()
        {
            StatusComboBox.ItemsSource = _database.GetStatuses();
        }

        private void LoadUsers()
        {
            AssignedUserComboBox.ItemsSource = _database.GetUsers();
        }

        private void LoadProjects()
        {
            ProjectComboBox.ItemsSource = _database.GetProjects();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            _database.UpdateTask(_task);
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
