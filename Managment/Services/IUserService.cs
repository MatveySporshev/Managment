using Managment.Models;

namespace ProjectManagementSystem
{
    public interface IUserService
    {
        void RegisterUser(User user);

        bool DeleteUser(string user);

        public User[] GetAllUsers();
    }
}
