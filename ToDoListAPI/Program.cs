using Microsoft.EntityFrameworkCore;
using ToDoListAPI;
using ToDoListAPI.Services;
using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register ToDoContext with SQL Server database.
builder.Services.AddDbContext<ToDoContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register TaskNlpService with your OpenAI API key
//builder.Services.AddSingleton(new TaskNlpService("YOUR_OPENAI_API_KEY")); // <-- Replace with your key
//builder.Services.AddSingleton(new OpenAIService()); // OLD manual set key

// Get OpenAI API key from configuration
var openAiApiKey = builder.Configuration["OpenAI:ApiKey"];
if (string.IsNullOrEmpty(openAiApiKey))
{
    throw new InvalidOperationException("OpenAI API key is not configured.");
}

// Register OpenAI service with API key from configuration
builder.Services.AddSingleton(new OpenAIService(openAiApiKey));

// Get Azure Service Bus connection string from configuration
var serviceBusConnectionString = builder.Configuration.GetConnectionString("AzureServiceBusConnection");
if (string.IsNullOrEmpty(serviceBusConnectionString))
{
    throw new InvalidOperationException("Azure Service Bus connection string is not configured.");
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