using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Core.Models;
using TradingMicroservice.Infrastructure.Configuration;

namespace TradingMicroservice.Infrastructure.Services;

public class KafkaService : IMessageQueueService
{
    private readonly IProducer<string, string> _producer;
    private readonly KafkaConfig _config;

    public KafkaService(IOptions<KafkaConfig> config)
    {
        _config = config.Value;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _config.BootstrapServers,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<string, string>(producerConfig).Build();
    }

    public async Task PublishTradeAsync(Trade trade)
    {
        try
        {
            var message = JsonSerializer.Serialize(trade);

            var result = await _producer.ProduceAsync(_config.Topic,
                new Message<string, string>
                {
                    Key = trade.Id.ToString(),
                    Value = message,
                    Timestamp = new Timestamp(DateTime.UtcNow)
                });

            if (result.Status == PersistenceStatus.Persisted)
            {
                Console.WriteLine($"Delivered trade message to: {result.TopicPartitionOffset}");
            }
        }
        catch (ProduceException<string, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            throw;
        }
    }

    public void Dispose()
    {
        _producer?.Dispose();
    }
}
