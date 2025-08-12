using Microsoft.EntityFrameworkCore;
using ToDoListAPI;
using ToDoListAPI.Services;
using MediatR;
using System.Reflection;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
builder.Services.AddSingleton(secretClient);

// Register ToDoContext with SQL Server database.
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register TaskNlpService with your OpenAI API key
//builder.Services.AddSingleton(new TaskNlpService("YOUR_OPENAI_API_KEY")); // <-- Replace with your key
//builder.Services.AddSingleton(new OpenAIService()); // OLD manual set key

// Get OpenAI API key from configuration -- from local this is old way
//var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
//if (string.IsNullOrEmpty(openAiApiKey))
//{
//    throw new InvalidOperationException("OpenAI API key is not configured.");
//}

// Get OpenAI API key from Azure Key Vault
string openAiApiKey;
try
{
    Console.WriteLine("Retrieving OpenAI API key from Azure Key Vault...");
    var openAiKeyResponse = await secretClient.GetSecretAsync("OpenAIApiKey");
    openAiApiKey = openAiKeyResponse.Value.Value;
    Console.WriteLine("Successfully retrieved OpenAI API key from Key Vault.");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to retrieve OpenAI API key from Key Vault: {ex.Message}");
    // Fallback to configuration
    openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
    if (string.IsNullOrEmpty(openAiApiKey))
    {
        throw new InvalidOperationException("OpenAI API key is not available from Key Vault or configuration.");
    }
    Console.WriteLine("Using OpenAI API key from configuration as fallback.");
}


// Register OpenAI service with API key from configuration
builder.Services.AddSingleton(new OpenAIService(openAiApiKey));

// Get Azure Service Bus connection string from configuration
//var serviceBusConnectionString = builder.Configuration.GetConnectionString("AzureServiceBusConnection");
//if (string.IsNullOrEmpty(serviceBusConnectionString))
//{
//    throw new InvalidOperationException("Azure Service Bus connection string is not configured.");
//}

string serviceBusConnectionString;
try
{
    Console.WriteLine("Retrieving ServiceBusConnection from Azure Key Vault...");
    var serviceBusConnectionResponse = await secretClient.GetSecretAsync("AzureServiceBusConnection");
    serviceBusConnectionString = serviceBusConnectionResponse.Value.Value;
    Console.WriteLine("Successfully retrieved ServiceBusConnection from Key Vault.");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to retrieve ServiceBusConnection from Key Vault: {ex.Message}");
    // Fallback to configuration
    serviceBusConnectionString = builder.Configuration["ConnectionStrings:AzureServiceBusConnection"];
    if (string.IsNullOrEmpty(serviceBusConnectionString))
    {
        throw new InvalidOperationException("ServiceBusConnection is not available from Key Vault or configuration.");
    }
    Console.WriteLine("Using ServiceBusConnection from configuration as fallback.");
}


// Register Azure Service Bus service
builder.Services.AddSingleton(serviceProvider =>
{
    var logger = serviceProvider.GetRequiredService<ILogger<AzureServiceBusService>>();
    return new AzureServiceBusService(serviceBusConnectionString, logger);
});

// Register MediatR for Event-Driven Architecture
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ToDoContext>();
    try
    {
        Console.WriteLine("Ensuring database is created...");
        await dbContext.Database.EnsureCreatedAsync();

        Console.WriteLine("Running database migrations...");
        await dbContext.Database.MigrateAsync();

        Console.WriteLine("Database setup completed successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error setting up database: {ex.Message}");
        // Don't throw - let the app continue, it might be a temporary connection issue
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policy => policy
    .WithOrigins("http://localhost:4200")
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Ensure proper cleanup of Service Bus resources
var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStopping.Register(async () =>
{
    var serviceBusService = app.Services.GetRequiredService<AzureServiceBusService>();
    await serviceBusService.DisposeAsync();
});

app.Run();