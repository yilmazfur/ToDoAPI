using MediatR;
using ToDoListAPI.Commands;
using ToDoListAPI.Events;

namespace ToDoListAPI.Handlers
{
    public class DeleteTodoHandler : IRequestHandler<DeleteTodoCommand, bool>
    {
        private readonly ToDoContext _context;
        private readonly IMediator _mediator;

        public DeleteTodoHandler(ToDoContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<bool> Handle(DeleteTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await _context.ToDos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (todo == null) return false;

            var taskName = todo.TaskName;
            _context.ToDos.Remove(todo);
            await _context.SaveChangesAsync(cancellationToken);

            // Publish domain event
            await _mediator.Publish(new TodoDeletedEvent(request.Id, taskName), cancellationToken);

            return true;
        }
    }
}