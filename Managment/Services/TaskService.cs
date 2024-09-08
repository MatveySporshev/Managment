using Newtonsoft.Json;
using ProjectManagementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managment.Models;
using Managment.Services;

namespace ProjectManagementSystem
{
    public class TaskService : ITaskService
    {
        private string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\tasks.json");

        private List<_Task> _tasks;
        private TaskLogService _taskLogService;

        public TaskService()
        {
            _tasks = LoadTasks();
            _taskLogService = new TaskLogService();
        }

        public void CreateTask(_Task task)
        {
            _tasks.Add(task);
            SaveTasks(); // Сохраняем задачи в JSON
            _taskLogService.LogTaskChange(task.ProjectId, task.Assignee, TaskStage.ToDo, task.Status);
        }

        public void UpdateTaskStatus(string taskId, TaskStage newStatus)
        {
            var task = _tasks.FirstOrDefault(t => t.ProjectId == taskId);
            if (task != null)
            {
                var oldStatus = task.Status;
                task.Status = newStatus;
                SaveTasks();
                // Логирование изменения статуса
                _taskLogService.LogTaskChange(taskId, task.Assignee, oldStatus, newStatus);
            }
        }

        public List<TaskLog> ViewTaskLogs(string taskId)
        {
            return _taskLogService.ViewLogsForTask(taskId);
        }

        public _Task[] GetTasksForUser(string username)
        {
            return _tasks.Where(t => t.Assignee == username).ToArray();
        }

        private List<_Task> LoadTasks()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);                 
                    return JsonConvert.DeserializeObject<List<_Task>>(json);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
                    return new List<_Task>();
                }
            }

            return new List<_Task>();
        }

        private void SaveTasks()
        {
            try
            {
                var json = JsonConvert.SerializeObject(_tasks, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                Console.WriteLine("Задачи успешно записаны.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи задач: {ex.Message}");
            }
        }


    }

}
