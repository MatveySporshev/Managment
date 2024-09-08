using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managment.Models
{
    public class TaskLog
    {
        public string TaskId { get; set; }
        public string Username { get; set; }
        public TaskStage OldStatus { get; set; }
        public TaskStage NewStatus { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
