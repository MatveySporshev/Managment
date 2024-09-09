namespace Managment.Models
{
    public class TaskLog
    {
        public Guid TaskId { get; set; }
        public string Username { get; set; }
        public WorkTaskStage OldStatus { get; set; }
        public WorkTaskStage NewStatus { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
