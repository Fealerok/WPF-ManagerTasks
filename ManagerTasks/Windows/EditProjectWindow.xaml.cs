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
            _database = Database.GetInstance();
            _project = project;


            DataContext = _project;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

            _database.UpdateProject(_project);
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
