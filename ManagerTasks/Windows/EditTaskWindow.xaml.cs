﻿using System;
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
using ManagerTasks.Classes;

namespace ManagerTasks.Windows
{

    public partial class EditTaskWindow : Window
    {
        private Database _database;
        private Classes.Task _task;

        public EditTaskWindow(Classes.Task task)
        {
            InitializeComponent();
            _database = Database.GetInstance(); 
            _task = task;


            DataContext = _task;


            LoadStatuses();
            LoadUsers();
            LoadProjects();
            LoadTeams();
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

        private void LoadTeams()
        {
            TeamComboBox.ItemsSource = _database.GetTeams();
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
