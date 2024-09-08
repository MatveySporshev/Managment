namespace Managment.Models
{
    public enum TaskStage
    {
        ToDo,
        InProgress,
        Done
    }
    public class _Task
    {
        public string ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Assignee { get; set; }
        public TaskStage Status { get; set; }
    }
}
