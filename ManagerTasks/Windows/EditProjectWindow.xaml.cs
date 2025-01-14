using ManagerTasks.Classes;
using System.Windows;

namespace ManagerTasks.Windows
{
    public partial class EditProjectWindow : Window
    {
        private Database _database;
        private Classes.Project _project;

        public EditProjectWindow(Classes.Project project)
        {
            InitializeComponent();
            _database = new Database(); // Инициализация базы данных
            _project = project;

            // Привязка данных проекта к элементам управления
            DataContext = _project;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение изменений в базе данных
            _database.UpdateProject(_project);
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
