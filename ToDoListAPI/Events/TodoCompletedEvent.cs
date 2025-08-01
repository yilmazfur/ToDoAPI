using MediatR;

namespace ToDoListAPI.Events
{
    public record TodoCompletedEvent(
        int TodoId,
        string TaskName,
        DateTime CompletedAt
    ) : INotification;
}