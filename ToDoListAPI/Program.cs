using Microsoft.EntityFrameworkCore;
using ToDoListAPI;
using ToDoListAPI.Services;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.Run();