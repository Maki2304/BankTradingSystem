using Microsoft.EntityFrameworkCore;
using TradingMicroservice.Infrastructure.Data;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Infrastructure.Services;
using TradingMicroservice.Infrastructure.Reposityory;
using TradingMicroservice.Infrastructure.Configuration;
using TradingMicroservice.Console.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TradingContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

builder.Services.AddScoped<ITradeRepository, TradeRepository>();
builder.Services.AddScoped<ITradingService, TradingService>();

builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection("Kafka"));

builder.Services.AddSingleton<IMessageQueueService, KafkaService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TradingContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();