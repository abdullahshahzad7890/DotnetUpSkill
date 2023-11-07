namespace TodoApp.Dto
{
    public class TodoListDto
    {

        public int Id { get; set; }
        public string Title { get; set; }
        public bool isCompleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
