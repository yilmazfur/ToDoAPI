using MediatR;
using ToDoListAPI.Events;

namespace ToDoListAPI.EventHandlers
{
    public class TodoUpdatedEventHandler : INotificationHandler<TodoUpdatedEvent>
    {
        private readonly ILogger<TodoUpdatedEventHandler> _logger;

        public TodoUpdatedEventHandler(ILogger<TodoUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(TodoUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Todo updated: {TodoId} - {TaskName}, Completed: {IsCompleted}", 
                notification.TodoId, notification.TaskName, notification.IsCompleted);
            
            await Task.CompletedTask;
        }
    }
}