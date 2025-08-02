using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace ToDoListAPI.Services
{
    public class AzureServiceBusService : IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ServiceBusSender _sender;
        private readonly ILogger<AzureServiceBusService> _logger;
        private const string TopicName = "todoqueue";

        public AzureServiceBusService(string connectionString, ILogger<AzureServiceBusService> logger)
        {
            _logger = logger;
            _client = new ServiceBusClient(connectionString);
            _sender = _client.CreateSender(TopicName);
        }

        public async Task PublishEventAsync<T>(T eventData, string eventType, CancellationToken cancellationToken = default)
        {
            try
            {
                var messageBody = JsonSerializer.Serialize(eventData);
                var message = new ServiceBusMessage(messageBody)
                {
                    Subject = eventType,
                    ContentType = "application/json",
                    CorrelationId = Guid.NewGuid().ToString()
                };

                // Add custom properties
                message.ApplicationProperties.Add("EventType", eventType);
                message.ApplicationProperties.Add("Timestamp", DateTimeOffset.UtcNow.ToString());

                await _sender.SendMessageAsync(message, cancellationToken);
                
                _logger.LogInformation("Published event {EventType} to Service Bus with correlation ID {CorrelationId}", 
                    eventType, message.CorrelationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event {EventType} to Service Bus", eventType);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_sender != null)
                await _sender.DisposeAsync();
            
            if (_client != null)
                await _client.DisposeAsync();
        }
    }
}