using MediatR;

namespace ToDoListAPI.Commands
{
    public record CreateTodoCommand(
        string TaskName,
        DateTime? Deadline = null
    ) : IRequest<ToDo>;
}