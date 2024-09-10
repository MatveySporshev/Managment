using Managment.Models;

namespace ProjectManagementSystem
{
    public interface ITaskService
    {
        void CreateTask(WorkTask task);
        void UpdateTaskStatus(string taskId, WorkTaskStage status);
        public WorkTask[] GetTasksForUser(string username);
        List<TaskLog> ViewTaskLogs(Guid taskId);
        public WorkTask[] GetAllTasks();

        bool DeleteTask(Guid taskId);

    }
}
