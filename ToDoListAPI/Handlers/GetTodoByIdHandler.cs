using MediatR;
using ToDoListAPI.Queries;

namespace ToDoListAPI.Handlers
{
    public class GetTodoByIdHandler : IRequestHandler<GetTodoByIdQuery, ToDo?>
    {
        private readonly ToDoContext _context;

        public GetTodoByIdHandler(ToDoContext context)
        {
            _context = context;
        }

        public async Task<ToDo?> Handle(GetTodoByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.ToDos.FindAsync(new object[] { request.Id }, cancellationToken);
        }
    }
}