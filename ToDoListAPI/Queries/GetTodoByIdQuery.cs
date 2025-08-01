using MediatR;

namespace ToDoListAPI.Queries
{
    public record GetTodoByIdQuery(int Id) : IRequest<ToDo?>;
}