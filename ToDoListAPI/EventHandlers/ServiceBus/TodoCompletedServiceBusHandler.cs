using MediatR;
using ToDoListAPI.Events;
using ToDoListAPI.Services;

namespace ToDoListAPI.EventHandlers.ServiceBus
{
    public class TodoCompletedServiceBusHandler : INotificationHandler<TodoCompletedEvent>
    {
        private readonly AzureServiceBusService _serviceBusService;
        private readonly ILogger<TodoCompletedServiceBusHandler> _logger;

        public TodoCompletedServiceBusHandler(AzureServiceBusService serviceBusService, ILogger<TodoCompletedServiceBusHandler> logger)
        {
            _serviceBusService = serviceBusService;
            _logger = logger;
        }

        public async Task Handle(TodoCompletedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _serviceBusService.PublishEventAsync(notification, "TodoCompleted", cancellationToken);
                _logger.LogInformation("Published TodoCompleted event to Service Bus for TodoId: {TodoId}", notification.TodoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish TodoCompleted event to Service Bus for TodoId: {TodoId}", notification.TodoId);
            }
        }
    }
}