using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp
{
    public class Seed
    {
        private readonly DataContext dataContext;
        public Seed(DataContext context)
        {
            this.dataContext = context;
        }
        public void SeedDataContext()
        {
            if (!dataContext.Todos.Any())
            {
                var Todos = new List<Todo>()
                {


                        new Todo()
                        {
                            Title = "Jack",
                            CreatedDate = new DateTime(2023,1,1),
                            UpdatedDate = new DateTime(2023,2,1),
                            isCompleted = true

                        }



                };
                dataContext.Todos.AddRange(Todos);
                dataContext.SaveChanges();
            }
        }
    }
}