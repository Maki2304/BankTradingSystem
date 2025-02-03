namespace TradingMicroservice.Infrastructure.Configuration;

public class KafkaConfig
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string Topic { get; set; } = "trades";
    public string GroupId { get; set; } = "trading-group";
    public int MessageTimeoutMs { get; set; } = 30000;
    public bool EnableIdempotence { get; set; } = true;
    public int NumPartitions { get; set; } = 3;
    public short ReplicationFactor { get; set; } = 1;
}
