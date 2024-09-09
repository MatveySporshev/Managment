using Managment.Models;
using Newtonsoft.Json;

namespace Managment.Services
{
    public class TaskLogService
    {
        private readonly string _logFilePath;

        public TaskLogService()
        {
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "task_logs.json");
        }

        public void LogTaskChange(string taskId, string username, WorkTaskStage oldStatus, WorkTaskStage newStatus)
        {
            if (Guid.TryParse(taskId, out Guid guid))
            {
                var logEntry = new TaskLog
                {
                    TaskId = guid,
                    Username = username,
                    OldStatus = oldStatus,
                    NewStatus = newStatus,
                    Timestamp = DateTime.Now
                };

                List<TaskLog> logs = LoadLogs();
                logs.Add(logEntry);
                SaveLogs(logs);
            }
            else
            {
                Console.WriteLine("\nНеверный формат Guid задачи.");
            }
        }

        public List<TaskLog> ViewLogsForTask(string taskId)
        {
            var logs = LoadLogs();
            return logs.Where(log => log.TaskId == Guid.Parse(taskId)).ToList();
        }

        private List<TaskLog> LoadLogs()
        {
            if (File.Exists(_logFilePath))
            {
                var json = File.ReadAllText(_logFilePath);
                return JsonConvert.DeserializeObject<List<TaskLog>>(json) ?? new List<TaskLog>();
            }
            return new List<TaskLog>();
        }

        private void SaveLogs(List<TaskLog> logs)
        {
            var json = JsonConvert.SerializeObject(logs, Formatting.Indented);
            File.WriteAllText(_logFilePath, json);
        }
    }
}
