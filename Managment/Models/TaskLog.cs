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
