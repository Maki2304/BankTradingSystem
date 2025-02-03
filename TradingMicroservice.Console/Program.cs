using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TradingMicroservice.Console.Services;
using TradingMicroservice.Infrastructure.Configuration;

namespace TradingMicroservice.Console;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<KafkaConfig>(
                    hostContext.Configuration.GetSection("Kafka"));

                services.AddHostedService<TradeConsumerService>();
            });
}