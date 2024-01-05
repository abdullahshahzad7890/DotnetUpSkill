using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Repository
{
    public class UserRepository : Interfaces.IUser
    {
        private readonly DataContext _dataContext;

        public UserRepository(DataContext context)
        {
            _dataContext = context;
        }

        public User GetUserByUsername(string username)
        {
            return _dataContext.User.FirstOrDefault(u => u.Username == username);
        }

        public bool UserExists(string username)
        {
            return _dataContext.User.Any(u => u.Username == username);
        }

        public bool CreateUser(User user)
        {
            _dataContext.User.Add(user);
            return Save();
        }

        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;        }
    }
}