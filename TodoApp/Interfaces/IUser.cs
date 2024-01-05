using TodoApp.Models;

namespace TodoApp.Interfaces
{
    public interface IUser
    {
        User GetUserByUsername(string username);
        Task<bool> UserExists(string username);
        Task<bool> CreateUser(User user);

    }
}
