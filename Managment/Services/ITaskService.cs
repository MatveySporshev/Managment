using Managment.Models;

namespace ProjectManagementSystem
{
    public interface ITaskService
    {
        void CreateTask(_Task task);
        void UpdateTaskStatus(string taskId, TaskStage status);
        public _Task[] GetTasksForUser(string username);
        List<TaskLog> ViewTaskLogs(string taskId);
    }
}
