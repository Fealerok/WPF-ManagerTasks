using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTasks.Classes
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public int StatusId { get; set; }
        public Status Status { get; set; }
        public int AssignedUserId { get; set; }
        public User AssignedUser { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int TeamId { get; set; } // Новое свойство
        public Team Team { get; set; } // Новое свойство
    }
}
