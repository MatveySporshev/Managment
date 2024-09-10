using Managment.Models;
using Managment.Services;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem
{
    public class TaskService : ITaskService
    {
        private readonly string _filePath = Path.Combine(Environment.CurrentDirectory, "tasks.json");

        private List<WorkTask> _tasks;
        private TaskLogService _taskLogService;
        private readonly IUserService _userService;

        public TaskService()
        {
            _tasks = LoadTasks();
            _taskLogService = new TaskLogService();
            
        }

        public void CreateTask(WorkTask task)
        {
            if (!_userService.UserExists(task.Assignee))
            {
                throw new ArgumentException("Невозможно назначить задачу на несуществующего сотрудника.");
            }
            else
            {
                _tasks.Add(task);
            SaveTasks();
            _taskLogService.LogTaskChange(task.ProjectId.ToString(), task.Assignee, WorkTaskStage.ToDo, task.Status);
            }
        }

        public void UpdateTaskStatus(string taskId, WorkTaskStage newStatus)
        {
            if (Guid.TryParse(taskId, out Guid guid))
            {
                var task = _tasks.FirstOrDefault(t => t.ProjectId == guid);
                if (task != null)
                {
                    var oldStatus = task.Status;
                    task.Status = newStatus;
                    SaveTasks();

                    _taskLogService.LogTaskChange(guid.ToString(), task.Assignee, oldStatus, newStatus);

                    Console.WriteLine("\nСтатус задачи успешно обновлен.");
                }
                else
                {
                    Console.WriteLine("\nЗадача с таким id не найдена.");
                }
            }
            else
            {
                Console.WriteLine("\nНеверный формат id задачи.");
            }
        }

        public List<TaskLog> ViewTaskLogs(Guid taskId)
        {

            return _taskLogService.ViewLogsForTask(taskId.ToString());

        }

        public WorkTask[] GetTasksForUser(string username)
        {
            return _tasks.Where(t => t.Assignee == username).ToArray();
        }

        private List<WorkTask> LoadTasks()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    var json = File.ReadAllText(_filePath);
                    return JsonConvert.DeserializeObject<List<WorkTask>>(json)
                        ?? new List<WorkTask>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при загрузке задач: {ex.Message}");
                    return new List<WorkTask>();
                }
            }

            return new List<WorkTask>();
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

        public WorkTask[] GetAllTasks()
        {
            return _tasks.ToArray();
        }

        public bool DeleteTask(Guid taskId)
        {
            var task = _tasks.FirstOrDefault(t => t.ProjectId == taskId);
            if (task != null)
            {
                _tasks.Remove(task);
                
                return true;
            }
            return false;
        }



    }

}
