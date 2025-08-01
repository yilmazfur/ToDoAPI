using MediatR;
using ToDoListAPI.Commands;
using ToDoListAPI.Events;

namespace ToDoListAPI.Handlers
{
    public class UpdateTodoHandler : IRequestHandler<UpdateTodoCommand, ToDo?>
    {
        private readonly ToDoContext _context;
        private readonly IMediator _mediator;

        public UpdateTodoHandler(ToDoContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<ToDo?> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = await _context.ToDos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (todo == null) return null;

            var wasCompleted = todo.IsCompleted;
            
            todo.TaskName = request.TaskName;
            todo.IsCompleted = request.IsCompleted;
            todo.Deadline = request.Deadline;

            await _context.SaveChangesAsync(cancellationToken);

            // Publish domain events
            await _mediator.Publish(new TodoUpdatedEvent(todo.Id, todo.TaskName, todo.IsCompleted, todo.Deadline, wasCompleted), cancellationToken);

            // If task was just completed, publish completion event
            if (!wasCompleted && todo.IsCompleted)
            {
                await _mediator.Publish(new TodoCompletedEvent(todo.Id, todo.TaskName, DateTime.UtcNow), cancellationToken);
            }

            return todo;
        }
    }
}