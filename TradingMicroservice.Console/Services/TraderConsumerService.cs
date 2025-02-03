using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TradingMicroservice.Core.Models;
using TradingMicroservice.Infrastructure.Configuration;
namespace TradingMicroservice.Console.Services;

public class TradeConsumerService : BackgroundService
    {
        private readonly IConsumer<string, string> _consumer;
        private readonly string _topic;
        private readonly ILogger<TradeConsumerService> _logger;

        public TradeConsumerService(
            IOptions<KafkaConfig> config,
            ILogger<TradeConsumerService> logger)
        {
            _logger = logger;

            var consumerConfig = new ConsumerConfig
            {
                BootstrapServers = config.Value.BootstrapServers,
                GroupId = config.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                AllowAutoCreateTopics = true
            };

            _consumer = new ConsumerBuilder<string, string>(consumerConfig).Build();
            _topic = config.Value.Topic;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Trade Consumer Service is starting.");

            _consumer.Subscribe(_topic);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = _consumer.Consume(stoppingToken);

                        var trade = JsonSerializer.Deserialize<Trade>(consumeResult.Message.Value);

                        _logger.LogInformation(
                            "Trade processed - ID: {Id}, Symbol: {Symbol}, Type: {Type}, " +
                            "Quantity: {Quantity}, Price: {Price}, Total: {Total}, Client: {ClientId}",
                            trade.Id,
                            trade.Symbol,
                            trade.Type,
                            trade.Quantity,
                            trade.Price,
                            trade.TotalAmount,
                            trade.ClientId);

                        _consumer.Commit(consumeResult);
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError(ex, "Error consuming message");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Trade Consumer Service is shutting down.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in Trade Consumer Service");
            }
            finally
            {
                _consumer.Close();
            }
        }

        public override void Dispose()
        {
            _consumer?.Dispose();
            base.Dispose();
        }
}
