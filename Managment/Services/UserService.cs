using Managment.Models;
using Newtonsoft.Json;

namespace ProjectManagementSystem
{
    public class UserService : IUserService
    {
        private string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\users.json");
        private List<User> _users;

        public UserService()
        {
            _users = LoadUsers();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        public void RegisterUser(User user)
        {
            _users.Add(user);
            Console.WriteLine($"Добавлен пользователь: {user.Username}");
            SaveUsers();
        }

        private List<User> LoadUsers()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var users = JsonConvert.DeserializeObject<List<User>>(json);
                //Console.WriteLine($"Загружено пользователей: {users?.Count ?? 0}");
                return users ?? new List<User>();
            }
            return new List<User>();
        }

        private void SaveUsers()
        {
            try
            {
                //Console.WriteLine($"Сохранение в файл: {Path.GetFullPath(_filePath)}");
                var json = JsonConvert.SerializeObject(_users, Formatting.Indented);
                File.WriteAllText(_filePath, json);
                //Console.WriteLine("Данные о пользователях успешно записаны.");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Ошибка при записи данных: {ex.Message}");
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Завершение программы... Сохранение данных.");
            SaveUsers();
            Console.WriteLine("Данные сохранены перед завершением программы.");
        }
    }


}
