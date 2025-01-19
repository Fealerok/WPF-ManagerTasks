using ManagerTasks.Classes;
using System;
using System.Windows;

namespace ManagerTasks.Windows
{
    public partial class Project : Window
    {
        private Database _database;

        public Project()
        {
            InitializeComponent();
            _database = Database.GetInstance();
            LoadProjects(); 
        }


        private void LoadProjects()
        {
            ProjectsGrid.ItemsSource = _database.GetProjects();
        }


        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var project = new Classes.Project
            {
                Name = "New Project",
            };

            _database.AddProject(project);
            LoadProjects(); 
        }


        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProject = ProjectsGrid.SelectedItem as Classes.Project;
            if (selectedProject != null)
            {

                var editProjectWindow = new EditProjectWindow(selectedProject);
                if (editProjectWindow.ShowDialog() == true)
                {
                    
                    LoadProjects();
                }
            }
            else
            {
                MessageBox.Show("Please select a project to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }


        private void DeleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProject = ProjectsGrid.SelectedItem as Classes.Project;
            if (selectedProject != null)
            {

                _database.DeleteProject(selectedProject.Id);
                LoadProjects(); 
            }
            else
            {
                MessageBox.Show("Please select a project to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
