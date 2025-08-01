using MediatR;
using ToDoListAPI.Events;
using ToDoListAPI.Services;

namespace ToDoListAPI.EventHandlers
{
    public class TodoCreatedEventHandler : INotificationHandler<TodoCreatedEvent>
    {
        private readonly ILogger<TodoCreatedEventHandler> _logger;
        private readonly OpenAIService _openAIService;

        public TodoCreatedEventHandler(ILogger<TodoCreatedEventHandler> logger, OpenAIService openAIService)
        {
            _logger = logger;
            _openAIService = openAIService;
        }

        public async Task Handle(TodoCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Todo created: {TodoId} - {TaskName}", notification.TodoId, notification.TaskName);

            try
            {
                var suggestions = await _openAIService.GetTaskSuggestions(notification.TaskName);
                if (suggestions != null)
                {
                    _logger.LogInformation("Generated {Count} AI suggestions for task: {TaskName}", 
                        suggestions.SuggestedTasks?.Count ?? 0, notification.TaskName);
                                        
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate AI suggestions for task: {TaskName}", notification.TaskName);
            }
        }
    }
}