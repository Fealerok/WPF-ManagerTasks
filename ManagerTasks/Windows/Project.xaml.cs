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
            LoadProjects(); // Загрузка проектов при открытии окна
        }

        // Загрузка проектов из базы данных
        private void LoadProjects()
        {
            ProjectsGrid.ItemsSource = _database.GetProjects();
        }

        // Обработчик кнопки "Add Project"
        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var project = new Classes.Project
            {
                Name = "New Project",
            };

            _database.AddProject(project);
            LoadProjects(); // Обновляем список проектов
        }

        // Обработчик кнопки "Edit Project"
        private void EditProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProject = ProjectsGrid.SelectedItem as Classes.Project;
            if (selectedProject != null)
            {
                // Открываем окно редактирования проекта
                var editProjectWindow = new EditProjectWindow(selectedProject);
                if (editProjectWindow.ShowDialog() == true)
                {
                    // Если пользователь нажал "Save", обновляем список проектов
                    LoadProjects();
                }
            }
            else
            {
                MessageBox.Show("Please select a project to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик кнопки "Delete Project"
        private void DeleteProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedProject = ProjectsGrid.SelectedItem as Classes.Project;
            if (selectedProject != null)
            {
                // Удаляем проект из базы данных
                _database.DeleteProject(selectedProject.Id);
                LoadProjects(); // Обновляем список проектов
            }
            else
            {
                MessageBox.Show("Please select a project to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Обработчик кнопки "Back to Tasks"
        private void BackToTasksButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно Tasks
            var tasksWindow = new Tasks();
            tasksWindow.Show(); // Показываем окно Tasks
            this.Close(); // Закрываем текущее окно Project
        }
    }
}
