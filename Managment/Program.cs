using Managment.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectManagementSystem
{
    class Program
    {
        static void Main(string[] args)
        {
            // Настройка DI-контейнера
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IAuthService, AuthService>()  // Singleton для AuthService
                .AddSingleton<IUserService, UserService>()  // Singleton для UserService
                .AddSingleton<ITaskService, TaskService>()  // Singleton для TaskService
                .BuildServiceProvider();

            // Внедрение зависимостей через конструктор
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
        }

        public void Run()
        {
            User currentUser = null;

            Console.WriteLine("Добро пожаловать в систему управления проектами");

            // Цикл аутентификации
            while (true)
            {
                if (currentUser == null)
                {
                    Console.WriteLine("\n1. Войти\n2. Завершить работу");
                    var option = Console.ReadLine();

                    if (option == "2") return;

                    Console.WriteLine("\nВведите имя пользователя:");
                    var username = Console.ReadLine().Trim();

                    Console.WriteLine("\nВведите пароль:");
                    var password = Console.ReadLine().Trim();

                    currentUser = _authService.Authenticate(username, password);

                    if (currentUser == null)
                    {
                        Console.WriteLine("\nНеверные учетные данные. Попробуйте снова.");
                    }
                    else
                    {
                        Console.WriteLine($"\nДобро пожаловать, {currentUser.Username}!");
                    }
                }
                else
                {
                    if (currentUser.Role == UserRole.Manager)
                    {
                        Console.WriteLine("\n1. Создать задачу\n2. Зарегистрировать сотрудника\n3. Просмотреть логи задач\n4. Выйти из системы\n5. Завершить работу");
                        var managerOption = Console.ReadLine();

                        if (managerOption == "1")
                        {
                            Console.WriteLine("\nВведите ID проекта:");
                            var projectId = Console.ReadLine().Trim();
                            Console.WriteLine("\nВведите название задачи:");
                            var title = Console.ReadLine().Trim();
                            Console.WriteLine("\nВведите описание задачи:");
                            var description = Console.ReadLine().Trim();
                            Console.WriteLine("\nНазначить задачу (имя пользователя сотрудника):");
                            var assignee = Console.ReadLine().Trim();

                            var task = new _Task
                            {
                                ProjectId = projectId,
                                Title = title,
                                Description = description,
                                Assignee = assignee,
                                Status = TaskStage.ToDo
                            };

                            _taskService.CreateTask(task);
                            Console.WriteLine("\nЗадача успешно создана.");
                        }
                        else if (managerOption == "2")
                        {
                            Console.WriteLine("\nВведите имя нового сотрудника:");
                            var newUsername = Console.ReadLine().Trim();
                            Console.WriteLine("\nВведите пароль нового сотрудника:");
                            var newPassword = Console.ReadLine().Trim();

                            _userService.RegisterUser(new User
                            {
                                Username = newUsername,
                                Password = newPassword,
                                Role = UserRole.Employee
                            });

                            Console.WriteLine("\nСотрудник успешно зарегистрирован.");
                        }
                        else if (managerOption == "3") // Просмотр логов задач
                        {
                            Console.WriteLine("\nВведите ID задачи для просмотра логов:");
                            var taskId = Console.ReadLine().Trim();

                            var logs = _taskService.ViewTaskLogs(taskId);
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
                        }
                        else if (managerOption == "4")
                        {
                            currentUser = null;
                            Console.WriteLine("\nВы вышли из системы.");
                            continue; // Вернуться к экрану логина
                        }
                        else if (managerOption == "5")
                        {
                            return; // Завершение работы приложения
                        }
                    }
                    else if (currentUser.Role == UserRole.Employee)
                    {
                        Console.WriteLine("\n1. Просмотреть задачи\n2. Изменить статус задачи\n3. Просмотреть логи задачи\n4. Выйти из системы\n5. Завершить работу");
                        var employeeOption = Console.ReadLine();

                        if (employeeOption == "1")
                        {
                            var tasks = _taskService.GetTasksForUser(currentUser.Username);
                            foreach (var task in tasks)
                            {
                                Console.WriteLine($"\n{task.ProjectId} - {task.Title} ({task.Description}): {task.Status}");
                            }
                        }
                        else if (employeeOption == "2")
                        {
                            Console.WriteLine("\nВведите ID задачи для обновления:");
                            var taskId = Console.ReadLine().Trim();
                            Console.WriteLine("\nВведите новый статус (ToDo, InProgress, Done):");
                            var status = (TaskStage)Enum.Parse(typeof(TaskStage), Console.ReadLine().Trim(), true);

                            _taskService.UpdateTaskStatus(taskId, status);
                            Console.WriteLine("\nСтатус задачи успешно обновлен.");
                        }
                        else if (employeeOption == "3") // Просмотр логов для сотрудника
                        {
                            Console.WriteLine("\nВведите ID задачи для просмотра логов:");
                            var taskId = Console.ReadLine().Trim();

                            var logs = _taskService.ViewTaskLogs(taskId);
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
                        }
                        else if (employeeOption == "4")
                        {
                            currentUser = null;
                            Console.WriteLine("\nВы вышли из системы.");
                            continue; // Вернуться к экрану логина
                        }
                        else if (employeeOption == "5")
                        {
                            return; // Завершение работы приложения
                        }
                    }
                }
            }
        }
    }

}
