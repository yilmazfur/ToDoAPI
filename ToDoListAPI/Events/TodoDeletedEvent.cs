using MediatR;

namespace ToDoListAPI.Events
{
    public record TodoDeletedEvent(
        int TodoId,
        string TaskName
    ) : INotification;
}