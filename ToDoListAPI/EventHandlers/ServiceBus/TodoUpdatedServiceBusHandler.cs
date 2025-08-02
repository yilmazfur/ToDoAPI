using MediatR;
using ToDoListAPI.Events;
using ToDoListAPI.Services;

namespace ToDoListAPI.EventHandlers.ServiceBus
{
    public class TodoUpdatedServiceBusHandler : INotificationHandler<TodoUpdatedEvent>
    {
        private readonly AzureServiceBusService _serviceBusService;
        private readonly ILogger<TodoUpdatedServiceBusHandler> _logger;

        public TodoUpdatedServiceBusHandler(AzureServiceBusService serviceBusService, ILogger<TodoUpdatedServiceBusHandler> logger)
        {
            _serviceBusService = serviceBusService;
            _logger = logger;
        }

        public async Task Handle(TodoUpdatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _serviceBusService.PublishEventAsync(notification, "TodoUpdated", cancellationToken);
                _logger.LogInformation("Published TodoUpdated event to Service Bus for TodoId: {TodoId}", notification.TodoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish TodoUpdated event to Service Bus for TodoId: {TodoId}", notification.TodoId);
            }
        }
    }
}