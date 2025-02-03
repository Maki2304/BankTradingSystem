using Microsoft.EntityFrameworkCore;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Core.Models;
using TradingMicroservice.Infrastructure.Data;

namespace TradingMicroservice.Infrastructure.Reposityory;

public class TradeRepository(TradingContext context) : ITradeRepository
{
    public async Task<Trade> GetByIdAsync(Guid id)
    {
        return await context.Trades.FindAsync(id);
    }

    public async Task<IEnumerable<Trade>> GetTradesAsync(string clientId = null, DateTime? from = null, DateTime? to = null)
    {
        var query = context.Trades.AsQueryable();

        if (!string.IsNullOrEmpty(clientId))
            query = query.Where(t => t.ClientId == clientId);

        if (from.HasValue)
            query = query.Where(t => t.ExecutionTime >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.ExecutionTime <= to.Value);

        return await query.OrderByDescending(t => t.ExecutionTime).ToListAsync();
    }

    public async Task<Trade> AddAsync(Trade trade)
    {
        await context.Trades.AddAsync(trade);
        await context.SaveChangesAsync();
        return trade;
    }

    public async Task UpdateAsync(Trade trade)
    {
        context.Entry(trade).State = EntityState.Modified;
        await context.SaveChangesAsync();
    }
}
