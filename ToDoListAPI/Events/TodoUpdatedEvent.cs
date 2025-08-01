using MediatR;

namespace ToDoListAPI.Events
{
    public record TodoUpdatedEvent(
        int TodoId,
        string TaskName,
        bool IsCompleted,
        DateTime? Deadline,
        bool WasCompleted
    ) : INotification;
}