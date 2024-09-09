using Managment.Models;
using Newtonsoft.Json;

namespace ProjectManagementSystem
{
    public class UserService : IUserService
    {
        private string _filePath = Path.Combine(Environment.CurrentDirectory, "users.json");
        private List<User> _users;

        public UserService()
        {
            _users = LoadUsers();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        public void RegisterUser(User user)
        {
            if (_users.Select(x => x.Username)
                .Contains(user.Username))
            {
                Console.WriteLine($"Пользователь с таким именем уже существует: {user.Username}");

                return;
            }

            _users.Add(user);
            Console.WriteLine($"Добавлен пользователь: {user.Username}");
            SaveUsers();
        }

        private List<User> LoadUsers()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                json = Utils.Decrypt(json);
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
                json = Utils.Encrypt(json);
                File.WriteAllText(_filePath, json);
                //Console.WriteLine("Данные о пользователях успешно записаны.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи данных: {ex.Message}");
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Завершение программы... Сохранение данных.");
            SaveUsers();
            Console.WriteLine("Данные сохранены перед завершением программы.");
        }

        public bool DeleteUser(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                _users.Remove(user);
                return true;
            }
            SaveUsers();
            Console.WriteLine($"Сотрудник {user.Username} удалён из программы");
            return false;
            
        }

        public User[] GetAllUsers()
        {
            return _users.ToArray();
        }
    }


}
