using MediatR;
using Microsoft.EntityFrameworkCore;
using ToDoListAPI.Queries;

namespace ToDoListAPI.Handlers
{
    public class GetAllTodosHandler : IRequestHandler<GetAllTodosQuery, IEnumerable<ToDo>>
    {
        private readonly ToDoContext _context;

        public GetAllTodosHandler(ToDoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ToDo>> Handle(GetAllTodosQuery request, CancellationToken cancellationToken)
        {
            return await _context.ToDos.ToListAsync(cancellationToken);
        }
    }
}