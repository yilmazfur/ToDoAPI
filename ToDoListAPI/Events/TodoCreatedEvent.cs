using MediatR;

namespace ToDoListAPI.Events
{
    public record TodoCreatedEvent(
        int TodoId,
        string TaskName,
        DateTime CreatedAt
    ) : INotification;
}