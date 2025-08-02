using MediatR;
using ToDoListAPI.Events;
using ToDoListAPI.Services;

namespace ToDoListAPI.EventHandlers.ServiceBus
{
    public class TodoCreatedServiceBusHandler : INotificationHandler<TodoCreatedEvent>
    {
        private readonly AzureServiceBusService _serviceBusService;
        private readonly ILogger<TodoCreatedServiceBusHandler> _logger;

        public TodoCreatedServiceBusHandler(AzureServiceBusService serviceBusService, ILogger<TodoCreatedServiceBusHandler> logger)
        {
            _serviceBusService = serviceBusService;
            _logger = logger;
        }

        public async Task Handle(TodoCreatedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _serviceBusService.PublishEventAsync(notification, "TodoCreated", cancellationToken);
                _logger.LogInformation("Published TodoCreated event to Service Bus for TodoId: {TodoId}", notification.TodoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish TodoCreated event to Service Bus for TodoId: {TodoId}", notification.TodoId);
                // Don't rethrow - we don't want Service Bus failures to break the main flow
            }
        }
    }
}