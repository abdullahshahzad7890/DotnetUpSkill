using TodoApp.Data;
using TodoApp.Interfaces;
using TodoApp.Models;

namespace TodoApp.Repository
{
    public class TodoListRepository : ITodoListRepository
    {
        private readonly DataContext _dataContext;
        public TodoListRepository(DataContext context)
        {
            _dataContext = context;
        }
        public bool CreateTodo(Todo todo)
        {
            _dataContext.Add(todo);

            return Save();
        }
        public bool DeleteTodo(Todo todo)
        {
            _dataContext.Remove(todo);
            return Save();
        }
        public Todo GetTodoList(int id)
        {
            var todo = _dataContext.Todos.FirstOrDefault(t => t.Id == id);
            if (todo == null)
            {
                throw new Exception("Todo not found.");
            }
            return todo;

        }
        public ICollection<Todo> GetTodos()
        {
            return _dataContext.Todos.OrderBy(x => x.Id).ToList();
        }
        public bool Save()
        {
            var saved = _dataContext.SaveChanges();
            return saved > 0 ? true : false;
        }
        public bool TodoExists(int todoId)
        {
            return _dataContext.Todos.Any(t => t.Id == todoId);
        }
        public bool UpdateTodo(Todo todo)
        {
            _dataContext.Update(todo);
            return Save();
        }
    }
}