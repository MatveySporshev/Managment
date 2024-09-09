namespace Managment.Models
{
    public enum WorkTaskStage
    {
        ToDo,
        InProgress,
        Done
    }
    public class WorkTask
    {
        public Guid ProjectId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Assignee { get; set; }
        public WorkTaskStage Status { get; set; }
    }
}
