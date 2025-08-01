using MediatR;
using ToDoListAPI.Events;

namespace ToDoListAPI.EventHandlers
{
    public class TodoCompletedEventHandler : INotificationHandler<TodoCompletedEvent>
    {
        private readonly ILogger<TodoCompletedEventHandler> _logger;

        public TodoCompletedEventHandler(ILogger<TodoCompletedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TodoCompletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Todo completed: {TodoId} - {TaskName} at {CompletedAt}", 
                notification.TodoId, notification.TaskName, notification.CompletedAt);
            
            await Task.CompletedTask;
        }
    }
}