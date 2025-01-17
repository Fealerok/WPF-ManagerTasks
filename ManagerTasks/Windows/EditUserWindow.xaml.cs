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
            _database = Database.GetInstance(); // Инициализация базы данных
            _user = user;

            // Привязка данных пользователя к элементам управления
            DataContext = _user;

            // Загрузка ролей и команд для ComboBox
            RoleComboBox.ItemsSource = _database.GetRoles();
            TeamComboBox.ItemsSource = _database.GetTeams();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Сохранение изменений в базе данных
            _database.UpdateUser(_user);
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