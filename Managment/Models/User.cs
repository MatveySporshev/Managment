namespace Managment.Models
{
    public enum UserRole
    {
        Manager,
        Employee
    }
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }
}
