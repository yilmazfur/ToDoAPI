using MediatR;

namespace ToDoListAPI.Commands
{
    public record UpdateTodoCommand(
        int Id,
        string TaskName,
        bool IsCompleted,
        DateTime? Deadline
    ) : IRequest<ToDo?>;
}