﻿using ManagerTasks.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ManagerTasks.Windows
{
    /// <summary>
    /// Логика взаимодействия для Tasks.xaml
    /// </summary>
    public partial class Tasks : Window
    {

        private Database _database;

        public Tasks()
        {
            InitializeComponent();
            _database = new Database(); // Инициализация базы данных
            LoadTasks(); // Загрузка задач при открытии окна
        }

        private void LoadTasks()
        {
            // Получаем задачи из базы данных и отображаем их в DataGrid
            TasksGrid.ItemsSource = _database.GetTasks();
        }

        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new Classes.Task
            {
                Title = "New Task",
                Description = "Description",
                DueDate = DateTime.Now,
                StatusId = 1, // Статус "В работе"
                AssignedUserId = 1, // Пример пользователя
                ProjectId = 0 // Пример проекта
            };

            _database.AddTask(task);
            LoadTasks(); // Обновляем список задач
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksGrid.SelectedItem as Classes.Task;
            if (selectedTask != null)
            {
                // Открываем окно редактирования задачи
                var edingTaskWindow = new EdingTask(selectedTask);
                if (edingTaskWindow.ShowDialog() == true)
                {
                    // Если пользователь нажал "Save", обновляем список задач
                    LoadTasks();
                }
            }
            else
            {
                MessageBox.Show("Please select a task to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksGrid.SelectedItem as Classes.Task;
            if (selectedTask != null)
            {
                _database.DeleteTask(selectedTask.Id);
                LoadTasks(); // Обновляем список задач
            }
        }

        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var projectWindow = new Project();
            projectWindow.Show(); // Показываем окно Tasks
            this.Close(); // Закрываем текущее окно Project
        }

        private void TeamsButton_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно Teams
            var teamsWindow = new Teams();
            teamsWindow.Show(); // Показываем окно Teams
            this.Close(); // Закрываем текущее окно Tasks
        }
    }
}
