using OpenAI.Chat;
using System.Text.Json;

namespace ToDoListAPI.Services
{
    public class OpenAIService
    {

        ChatClient client;
        public OpenAIService(string apiKey)
        {
            client = new ChatClient(model: "gpt-4o-mini", apiKey: apiKey);
        }

        public async Task<string> GetResponseFromAI(string question)
        {
            ChatCompletionOptions chatCompletionOptions = new ChatCompletionOptions();
            chatCompletionOptions.MaxOutputTokenCount = 150;
            //var response = await client.CompleteChatAsync("What is the capital of NL", new ChatCompletionOptions { MaxOutputTokenCount = 5 });
            var response = await client.CompleteChatAsync(question);         
            return response.Value.Content[0].Text;
        }

        public async Task<TaskSuggestions?> GetTaskSuggestions(string task)
        {
            var prompt = $@"Break down the task '{task}' into 2 smaller actionable subtasks. 
Return response in this exact JSON format:
{{
    ""originalTask"": ""{task}"",
    ""suggestedTasks"": [""subtask1"", ""subtask2""]
}}";

            try
            {
                var response = await client.CompleteChatAsync(prompt);
                var jsonResponse = response.Value.Content[0].Text;

                var taskSuggestions = JsonSerializer.Deserialize<TaskSuggestions>(jsonResponse,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return taskSuggestions;
            }
            catch (JsonException)
            {
                return null;
            }
        }
    }
}
