using MediatR;
using ToDoListAPI.Events;
using ToDoListAPI.Services;

namespace ToDoListAPI.EventHandlers.ServiceBus
{
    public class TodoDeletedServiceBusHandler : INotificationHandler<TodoDeletedEvent>
    {
        private readonly AzureServiceBusService _serviceBusService;
        private readonly ILogger<TodoDeletedServiceBusHandler> _logger;

        public TodoDeletedServiceBusHandler(AzureServiceBusService serviceBusService, ILogger<TodoDeletedServiceBusHandler> logger)
        {
            _serviceBusService = serviceBusService;
            _logger = logger;
        }

        public async Task Handle(TodoDeletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _serviceBusService.PublishEventAsync(notification, "TodoDeleted", cancellationToken);
                _logger.LogInformation("Published TodoDeleted event to Service Bus for TodoId: {TodoId}", notification.TodoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish TodoDeleted event to Service Bus for TodoId: {TodoId}", notification.TodoId);
            }
        }
    }
}