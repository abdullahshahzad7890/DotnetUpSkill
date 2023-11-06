using TodoApp.Models;

namespace TodoApp.Interfaces
{
    public interface ITodoListRepository
    {
        ICollection<Todo> GetTodos();
        Todo GetTodoList(int id);
        bool TodoExists(int id);    
        bool CreateTodo(Todo todo);
        bool UpdateTodo(Todo todo);
        bool DeleteTodo(Todo todo);    
        bool Save();  
    }
}