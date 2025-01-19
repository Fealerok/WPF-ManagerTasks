using ManagerTasks.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ManagerTasks.Windows
{
    public partial class Tasks : Window
    {
        private Database _database;
        private string _UserRole;
        private List<Classes.Task> _allTasks; 
        private List<Classes.Task> _filteredTasks; 

        public Tasks()
        {
            InitializeComponent();
            _database = Database.GetInstance();
            _UserRole = _database.GetUserRole();

            if (_UserRole == "Администратор") UsersButton.Visibility = Visibility.Visible;
            else UsersButton.Visibility = Visibility.Hidden;

            LoadTasks(); 
            LoadFilters(); 


            FilterComboBox.SelectionChanged += FilterComboBox_SelectionChanged;
        }

        private void LoadTasks()
        {

            _allTasks = _database.GetTasks();
            _filteredTasks = _allTasks; 
            TasksGrid.ItemsSource = _filteredTasks;
        }

        private void LoadFilters()
        {

            var statuses = _database.GetStatuses();
            var users = _database.GetUsers();
            var projects = _database.GetProjects();


            FilterValueComboBox.ItemsSource = statuses;
        }

        private void FilterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            switch (selectedFilter)
            {
                case "По статусу":
                    FilterValueComboBox.DisplayMemberPath = "Name"; 
                    FilterValueComboBox.ItemsSource = _database.GetStatuses();
                    FilterValueComboBox.Visibility = Visibility.Visible;
                    FilterDatePicker.Visibility = Visibility.Collapsed;
                    break;

                case "По дате":
                    FilterValueComboBox.Visibility = Visibility.Collapsed;
                    FilterDatePicker.Visibility = Visibility.Visible;
                    break;

                case "По исполнителю":
                    FilterValueComboBox.DisplayMemberPath = "Username"; 
                    FilterValueComboBox.ItemsSource = _database.GetUsers();
                    FilterValueComboBox.Visibility = Visibility.Visible;
                    FilterDatePicker.Visibility = Visibility.Collapsed;
                    break;

                case "По проекту":
                    FilterValueComboBox.DisplayMemberPath = "Name"; 
                    FilterValueComboBox.ItemsSource = _database.GetProjects();
                    FilterValueComboBox.Visibility = Visibility.Visible;
                    FilterDatePicker.Visibility = Visibility.Collapsed;
                    break;

                default:
                    FilterValueComboBox.Visibility = Visibility.Collapsed;
                    FilterDatePicker.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedFilter = (FilterComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            var filteredTasks = _allTasks;

            switch (selectedFilter)
            {
                case "По статусу":

                    if (FilterValueComboBox.SelectedValue == null) MessageBox.Show("Выберите статус");
                    else
                    {
                        int selectedStatusId = (int)FilterValueComboBox.SelectedValue;
                        if (selectedStatusId > 0)
                        {
                            filteredTasks = filteredTasks.Where(t => t.StatusId == selectedStatusId).ToList();
                        }
                    }
                    
                    break;


                case "По дате":
                    
                    var selectedDate = FilterDatePicker.SelectedDate;
                    if (selectedDate.HasValue)
                    {
                        filteredTasks = filteredTasks.Where(t => t.DueDate.Date == selectedDate.Value.Date).ToList();
                    }
                    break;

                case "По исполнителю":
                    if (FilterValueComboBox.SelectedValue == null) MessageBox.Show("Выберите исполнителя");
                    else
                    {
                        int selectedUserId = (int)FilterValueComboBox.SelectedValue;
                        if (selectedUserId > 0)
                        {
                            filteredTasks = filteredTasks.Where(t => t.AssignedUserId == selectedUserId).ToList();
                        }
                    }
                
                    break;

                case "По проекту":
                    if (FilterValueComboBox.SelectedValue == null) MessageBox.Show("Выберите проект");
                    else
                    {
                        int selectedProjectId = (int)FilterValueComboBox.SelectedValue;
                        if ((int)selectedProjectId > 0)
                        {
                            filteredTasks = filteredTasks.Where(t => t.ProjectId == (int)selectedProjectId).ToList();
                        }
                    }
                   
                    break;
            }

            TasksGrid.ItemsSource = filteredTasks;
        }

        private void ResetFilterButton_Click(object sender, RoutedEventArgs e)
        {

            TasksGrid.ItemsSource = _allTasks;
            FilterComboBox.SelectedIndex = -1;
            FilterValueComboBox.SelectedIndex = -1;
            FilterDatePicker.SelectedDate = null;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            ApplySearch();
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {

            SearchTextBox.Text = string.Empty;
            ApplySearch();
        }

        private void ApplySearch()
        {
            var searchText = SearchTextBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {

                TasksGrid.ItemsSource = _filteredTasks;
            }
            else
            {

                var searchedTasks = _filteredTasks
                    .Where(t => (t.Title?.ToLower().Contains(searchText) ?? false) ||
                                (t.Description?.ToLower().Contains(searchText) ?? false) ||
                                (t.Status?.Name?.ToLower().Contains(searchText) ?? false) ||
                                (t.AssignedUser?.Username?.ToLower().Contains(searchText) ?? false) ||
                                (t.Project?.Name?.ToLower().Contains(searchText) ?? false) ||
                                (t.Team?.Name?.ToLower().Contains(searchText) ?? false))
                    .ToList();

                TasksGrid.ItemsSource = searchedTasks;
            }
        }



        private void AddTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var task = new Classes.Task
            {
                Title = "New Task",
                Description = "Description",
                DueDate = DateTime.Now,
                StatusId = 1, 
                AssignedUserId = _database.GetIdLoggedUser(_database.Username), 
                ProjectId = 0 
            };

            _database.AddTask(task);
            LoadTasks(); 
        }

        private void EditTaskButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedTask = TasksGrid.SelectedItem as Classes.Task;
            if (selectedTask != null)
            {

                var edingTaskWindow = new EditTaskWindow(selectedTask);
                if (edingTaskWindow.ShowDialog() == true)
                {

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
                LoadTasks(); 
            }
        }

        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            var projectWindow = new Project();
            projectWindow.Show(); 
            this.Close(); 
        }

        private void TeamsButton_Click(object sender, RoutedEventArgs e)
        {
  
            var teamsWindow = new Teams();
            teamsWindow.Show(); 
            this.Close(); 
        }

        private void UsersButton_Click(object sender, RoutedEventArgs e)
        {
            var usersWindow = new Users();
            usersWindow.Show(); 
            this.Close(); 
        }

        private void PersonalAccountButton_Click(object sender, RoutedEventArgs e)
        {

            var personalAccountWindow = new PersonalAccountWindow();
            personalAccountWindow.Show(); 
            this.Close(); 
        }

        private void TasksGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
