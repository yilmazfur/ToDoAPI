using MediatR;

namespace ToDoListAPI.Queries
{
    public record GetAllTodosQuery() : IRequest<IEnumerable<ToDo>>;
}