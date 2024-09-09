using Managment.Models;
using Newtonsoft.Json;

namespace ProjectManagementSystem
{
    public class AuthService : IAuthService
    {
        private readonly string UserFile = "users.json";

        public User Authenticate(string username, string password)
        {
            var users = LoadUsers();
            return users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        private List<User> LoadUsers()
        {
            if (!File.Exists(UserFile)) return new List<User>();
            var json = File.ReadAllText(UserFile);
            json = Utils.Decrypt(json);
            return JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
        }
    }
}
