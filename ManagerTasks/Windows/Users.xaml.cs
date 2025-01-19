using ManagerTasks.Classes;
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

    public partial class Users : Window
    {
        private Database _database;

        public Users()
        {
            InitializeComponent();
            _database = Database.GetInstance(); 
            LoadUsers(); 
        }

      
        private void LoadUsers()
        {
            UsersGrid.ItemsSource = _database.GetUsersWithDetails();
        }


      
        private void EditUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersGrid.SelectedItem as User;
            if (selectedUser != null)
            {
             
                var editUserWindow = new EditUserWindow(selectedUser);
                if (editUserWindow.ShowDialog() == true)
                {
                    
                    LoadUsers();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

     
        private void DeleteUserButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsersGrid.SelectedItem as User;
            if (selectedUser != null)
            {
    
                _database.DeleteUser(selectedUser.Id);
                LoadUsers(); 
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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
