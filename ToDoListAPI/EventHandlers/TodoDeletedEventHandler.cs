using MediatR;
using ToDoListAPI.Events;

namespace ToDoListAPI.EventHandlers
{
    public class TodoDeletedEventHandler : INotificationHandler<TodoDeletedEvent>
    {
        private readonly ILogger<TodoDeletedEventHandler> _logger;

        public TodoDeletedEventHandler(ILogger<TodoDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TodoDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Todo deleted: {TodoId} - {TaskName}", notification.TodoId, notification.TaskName);            
            
            await Task.CompletedTask;
        }
    }
}