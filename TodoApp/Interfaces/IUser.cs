using TodoApp.Models;

namespace TodoApp.Interfaces
{
    public interface IUser
    {
        User GetUserByUsername(string username);
        bool UserExists(string username);
        bool CreateUser(User user);
        bool Save();

    }
}
