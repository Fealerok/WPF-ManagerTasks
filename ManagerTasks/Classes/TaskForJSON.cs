using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerTasks.Classes
{
    public class TaskForJSON
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public string AssignedUser { get; set; }
        public string Project { get; set; }
        public string Team { get; set; }
    }
}
