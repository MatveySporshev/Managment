using ProjectManagementSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Managment.Models;
using Newtonsoft.Json;

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
