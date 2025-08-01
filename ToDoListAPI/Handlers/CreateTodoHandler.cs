using MediatR;
using ToDoListAPI.Commands;
using ToDoListAPI.Events;

namespace ToDoListAPI.Handlers
{
    public class CreateTodoHandler : IRequestHandler<CreateTodoCommand, ToDo>
    {
        private readonly ToDoContext _context;
        private readonly IMediator _mediator;

        public CreateTodoHandler(ToDoContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<ToDo> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
        {
            var todo = new ToDo
            {
                TaskName = request.TaskName,
                Deadline = request.Deadline,
                IsCompleted = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.ToDos.Add(todo);
            await _context.SaveChangesAsync(cancellationToken);

            // Publish domain event
            await _mediator.Publish(new TodoCreatedEvent(todo.Id, todo.TaskName, todo.CreatedAt), cancellationToken);

            return todo;
        }
    }
}