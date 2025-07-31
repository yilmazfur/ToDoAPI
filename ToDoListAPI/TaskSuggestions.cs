namespace ToDoListAPI
{
    public class TaskSuggestions
    {
        public required string OriginalTask { get; set; }
        public List<string>? SuggestedTasks { get; set; }
    }
}
