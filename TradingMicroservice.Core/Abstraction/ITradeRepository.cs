using TradingMicroservice.Core.Models;

namespace TradingMicroservice.Core.Abstraction;

public interface ITradeRepository
{
    Task<Trade> GetByIdAsync(Guid id);
    Task<IEnumerable<Trade>> GetTradesAsync(string clientId = null, DateTime? from = null, DateTime? to = null);
    Task<Trade> AddAsync(Trade trade);
    Task UpdateAsync(Trade trade);
}
