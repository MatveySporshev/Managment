using Managment.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managment.Services
{
    public class TaskLogService
    {
        private readonly string _logFilePath;

        public TaskLogService()
        {
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "task_logs.json");
        }

        public void LogTaskChange(string taskId, string username, TaskStage oldStatus, TaskStage newStatus)
        {
            var logEntry = new TaskLog
            {
                TaskId = taskId,
                Username = username,
                OldStatus = oldStatus,
                NewStatus = newStatus,
                Timestamp = DateTime.Now
            };

            List<TaskLog> logs = LoadLogs();
            logs.Add(logEntry);
            SaveLogs(logs);
        }

        public List<TaskLog> ViewLogsForTask(string taskId)
        {
            var logs = LoadLogs();
            return logs.Where(log => log.TaskId == taskId).ToList();
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
