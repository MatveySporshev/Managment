using Managment.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<IUserService, UserService>()
                .AddSingleton<ITaskService, TaskService>()
                .BuildServiceProvider();

            var app = new ProjectManagementApp(
                serviceProvider.GetService<IAuthService>(),
                serviceProvider.GetService<IUserService>(),
                serviceProvider.GetService<ITaskService>());

            app.Run();
        }
    }

    public class ProjectManagementApp
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;
        private readonly ITaskService _taskService;

        public ProjectManagementApp(IAuthService authService, IUserService userService, ITaskService taskService)
        {
            _authService = authService;
            _userService = userService;
            _taskService = taskService;

            GenerateStartData();
        }

        public void Run()
        {
            User currentUser = null;

            Console.WriteLine("Добро пожаловать в систему управления проектами");

            while (true)
            {
                if (currentUser == null)
                {
                    Console.WriteLine("\n1. Войти\n2. Завершить работу");
                    var option = Console.ReadLine().Trim();

                    switch (option)
                    {
                        case "1":
                            Console.WriteLine("\nВведите имя пользователя:");
                            var username = Console.ReadLine().Trim();
                            if (string.IsNullOrWhiteSpace(username))
                            {
                                Console.WriteLine("Имя пользователя не может быть пустым. Попробуйте снова.");
                                continue;
                            }

                            Console.WriteLine("\nВведите пароль:");
                            var password = Console.ReadLine().Trim();
                            if (string.IsNullOrWhiteSpace(password))
                            {
                                Console.WriteLine("Пароль не может быть пустым. Попробуйте снова.");
                                continue;
                            }

                            currentUser = _authService.Authenticate(username, password);

                            if (currentUser == null)
                            {
                                Console.WriteLine("\nНеверные учетные данные. Попробуйте снова.");
                            }
                            else
                            {
                                Console.WriteLine($"\nДобро пожаловать, {currentUser.Username}!");
                            }
                            break;

                        case "2":
                            return;

                        default:
                            Console.WriteLine("\nВведены неверные данные. Попробуйте снова.");
                            break;
                    }
                }
                else
                {
                    switch (currentUser.Role)
                    {
                        case UserRole.Manager:
                            Console.WriteLine("\n1. Создать задачу\n2. Зарегистрировать сотрудника\n3. Просмотреть логи задач\n4. Просмотреть все задачи\n5. Просмотреть всех сотрудников\n6. Удалить сотрудника\n7. Выйти из системы\n8. Завершить работу");
                            var managerOption = Console.ReadLine().Trim();

                            switch (managerOption)
                            {
                                case "1":
                                    Console.WriteLine("\nВведите название задачи:");
                                    var title = Console.ReadLine().Trim();
                                    if (string.IsNullOrWhiteSpace(title))
                                    {
                                        Console.WriteLine("Название задачи не может быть пустым. Попробуйте снова.");
                                        continue;
                                    }

                                    Console.WriteLine("\nВведите описание задачи:");
                                    var description = Console.ReadLine().Trim();
                                    if (string.IsNullOrWhiteSpace(description))
                                    {
                                        Console.WriteLine("Описание задачи не может быть пустым. Попробуйте снова.");
                                        continue;
                                    }

                                    Console.WriteLine("\nНазначить задачу (имя пользователя сотрудника):");
                                    var assignee = Console.ReadLine().Trim();
                                    if (string.IsNullOrWhiteSpace(assignee))
                                    {
                                        Console.WriteLine("Имя пользователя не может быть пустым. Попробуйте снова.");
                                        continue;
                                    }

                                    var task = new WorkTask
                                    {
                                        ProjectId = Guid.NewGuid(),
                                        Title = title,
                                        Description = description,
                                        Assignee = assignee,
                                        Status = WorkTaskStage.ToDo
                                    };

                                    _taskService.CreateTask(task);
                                    Console.WriteLine("\nЗадача успешно создана.");
                                    break;

                                case "2":
                                    Console.WriteLine("\nВведите имя нового сотрудника:");
                                    var newUsername = Console.ReadLine().Trim();
                                    if (string.IsNullOrWhiteSpace(newUsername))
                                    {
                                        Console.WriteLine("Имя нового сотрудника не может быть пустым. Попробуйте снова.");
                                        continue;
                                    }

                                    Console.WriteLine("\nВведите пароль нового сотрудника:");
                                    var newPassword = Console.ReadLine().Trim();
                                    if (string.IsNullOrWhiteSpace(newPassword))
                                    {
                                        Console.WriteLine("Пароль не может быть пустым. Попробуйте снова.");
                                        continue;
                                    }

                                    _userService.RegisterUser(new User
                                    {
                                        Username = newUsername,
                                        Password = newPassword,
                                        Role = UserRole.Employee
                                    });
                                    Console.WriteLine("\nСотрудник успешно зарегистрирован.");
                                    break;

                                case "3":
                                    Console.WriteLine("\nВведите ID задачи для просмотра логов:");
                                    var taskId = Console.ReadLine().Trim();

                                    if (!Guid.TryParse(taskId, out var taskGuid))
                                    {
                                        Console.WriteLine("Неверный формат ID задачи. Попробуйте снова.");
                                        continue;
                                    }

                                    var logs = _taskService.ViewTaskLogs(taskGuid);
                                    if (logs.Count == 0)
                                    {
                                        Console.WriteLine("\nЛоги для данной задачи отсутствуют.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"\nЛоги для задачи {taskId}:");
                                        foreach (var log in logs)
                                        {
                                            Console.WriteLine($"{log.Timestamp}: {log.Username} изменил статус с {log.OldStatus} на {log.NewStatus}");
                                        }
                                    }
                                    break;

                                case "4": // Просмотр всех задач
                                    var allTasks = _taskService.GetAllTasks();
                                    if (allTasks.Length == 0)
                                    {
                                        Console.WriteLine("\nЗадачи отсутствуют.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nВсе задачи:");
                                        foreach (var taskItem in allTasks)
                                        {
                                            Console.WriteLine($"\n{taskItem.ProjectId} - {taskItem.Title} ({taskItem.Description}): {taskItem.Status}, назначено: {taskItem.Assignee}");
                                        }
                                    }
                                    break;

                                case "5": // Просмотр всех сотрудников
                                    var allUsers = _userService.GetAllUsers();
                                    if (allUsers.Length == 0)
                                    {
                                        Console.WriteLine("\nСотрудники отсутствуют.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nВсе сотрудники:");
                                        foreach (var user in allUsers)
                                        {
                                            Console.WriteLine($"Имя пользователя: {user.Username}, Роль: {user.Role}");
                                        }
                                    }
                                    break;

                                case "6": // Удаление сотрудника
                                    Console.WriteLine("\nВведите имя пользователя для удаления:");
                                    var usernameToDelete = Console.ReadLine().Trim();
                                    if (string.IsNullOrWhiteSpace(usernameToDelete))
                                    {
                                        Console.WriteLine("Имя пользователя не может быть пустым. Попробуйте снова.");
                                        continue;
                                    }
                                    
                                    if (_userService.DeleteUser(usernameToDelete))
                                    {
                                        Console.WriteLine("\nПользователь успешно удален.");
                                    }
                                    else
                                    {
                                        Console.WriteLine("\nПользователь не найден.");
                                    }
                                    break;

                                case "7":
                                    currentUser = null;
                                    Console.WriteLine("\nВы вышли из системы.");
                                    break;

                                case "8":
                                    return;

                                default:
                                    Console.WriteLine("\nНеверная опция, попробуйте снова.");
                                    break;
                            }
                            break;

                        case UserRole.Employee:
                            Console.WriteLine("\n1. Просмотреть задачи\n2. Изменить статус задачи\n3. Просмотреть логи задачи\n4. Выйти из системы\n5. Завершить работу");
                            var employeeOption = Console.ReadLine().Trim();

                            switch (employeeOption)
                            {
                                case "1":
                                    var tasks = _taskService.GetTasksForUser(currentUser.Username);
                                    foreach (var task in tasks)
                                    {
                                        Console.WriteLine($"\n{task.ProjectId} - {task.Title} ({task.Description}): {task.Status}");
                                    }

                                    if (tasks.Length == 0)
                                        Console.WriteLine("\nУ вас нет задач.");
                                    break;

                                case "2":
                                    Console.WriteLine("\nВведите ID задачи для обновления:");
                                    var taskIdToUpdate = Console.ReadLine().Trim();

                                    if (!Guid.TryParse(taskIdToUpdate, out var taskGuidToUpdate))
                                    {
                                        Console.WriteLine("Неверный формат ID задачи. Попробуйте снова.");
                                        continue;
                                    }

                                    Console.WriteLine("\nВведите новый статус (ToDo, InProgress, Done):");
                                    var statusInput = Console.ReadLine().Trim();
                                    if (!Enum.TryParse<WorkTaskStage>(statusInput, true, out var status) ||
                                        !(status == WorkTaskStage.ToDo || status == WorkTaskStage.InProgress || status == WorkTaskStage.Done))
                                    {
                                        Console.WriteLine("Неверный статус задачи, попробуйте снова.");
                                        continue;
                                    }

                                    _taskService.UpdateTaskStatus(taskGuidToUpdate.ToString(), status);
                                    Console.WriteLine("\nСтатус задачи обновлен.");
                                    break;

                                case "3":
                                    Console.WriteLine("\nВведите ID задачи для просмотра логов:");
                                    var taskIdToViewLogs = Console.ReadLine().Trim();

                                    if (!Guid.TryParse(taskIdToViewLogs, out var taskGuidToViewLogs))
                                    {
                                        Console.WriteLine("Неверный формат ID задачи. Попробуйте снова.");
                                        continue;
                                    }

                                    var taskLogs = _taskService.ViewTaskLogs(taskGuidToViewLogs);
                                    if (taskLogs.Count == 0)
                                    {
                                        Console.WriteLine("\nЛоги для данной задачи отсутствуют.");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"\nЛоги для задачи {taskIdToViewLogs}:");
                                        foreach (var log in taskLogs)
                                        {
                                            Console.WriteLine($"{log.Timestamp}: {log.Username} изменил статус с {log.OldStatus} на {log.NewStatus}");
                                        }
                                    }
                                    break;

                                case "4":
                                    currentUser = null;
                                    Console.WriteLine("\nВы вышли из системы.");
                                    break;

                                case "5":
                                    return;

                                default:
                                    Console.WriteLine("\nНеверная опция, попробуйте снова.");
                                    break;
                            }
                            break;

                        default:
                            Console.WriteLine("Неизвестная роль пользователя.");
                            break;
                    }
                }
            }
        }

        private void GenerateStartData()
        {
            _userService.RegisterUser(new User
            {
                Username = "manager",
                Password = "123",
                Role = UserRole.Manager
            });
        }
    }
}
