namespace ToDoListAPI
{
    public class ToDo
    {
        public int Id { get; set; } // Unique identifier
        public required string TaskName { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp for when the task was created
        public DateTime? Deadline { get; set; }
    }
}
