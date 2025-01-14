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
            _database = new Database(); // Инициализация базы данных
            _task = task;

            // Привязка данных задачи к элементам управления
            DataContext = _task;

            // Загрузка данных для ComboBox
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
            // Сохранение изменений в базе данных
            _database.UpdateTask(_task);
            DialogResult = true; // Закрыть окно с результатом "ОК"
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрыть окно с результатом "Отмена"
            Close();
        }
    }
}
