namespace ToDoListAPI
{
    public class ToDo
    {
        public int Id { get; set; } // Unique identifier
        public required string TaskName { get; set; }
        public bool IsCompleted { get; set; }
    }
}
