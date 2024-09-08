using Managment.Models;

namespace ProjectManagementSystem
{
    public interface IAuthService
    {
        User Authenticate(string username, string password);
    }
}
