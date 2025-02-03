using TradingMicroservice.Core.Models;

namespace TradingMicroservice.Core.Abstraction;

public interface IMessageQueueService
{
    Task PublishTradeAsync(Trade trade);
}
