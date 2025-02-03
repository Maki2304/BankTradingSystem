using TradingMicroservice.Core.Models;

namespace TradingMicroservice.Core.Abstraction;

public interface ITradingService
{
    Task<Trade> ExecuteTradeAsync(TradeRequest request);
    Task<IEnumerable<Trade>> GetTradesAsync(string clientId = null, DateTime? from = null, DateTime? to = null);
    Task<Trade> GetTradeByIdAsync(Guid id);
}
