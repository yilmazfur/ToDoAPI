using MediatR;

namespace ToDoListAPI.Commands
{
    public record DeleteTodoCommand(int Id) : IRequest<bool>;
}