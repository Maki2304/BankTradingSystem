using Microsoft.Extensions.Logging;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Core.Enums;
using TradingMicroservice.Core.Models;

namespace TradingMicroservice.Infrastructure.Services;

public class TradingService : ITradingService
{
    private readonly ITradeRepository _tradeRepository;
    private readonly IMessageQueueService _messageQueueService;
    private readonly ILogger<TradingService> _logger;

    public TradingService(
        ITradeRepository tradeRepository,
        IMessageQueueService messageQueueService,
        ILogger<TradingService> logger)
    {
        _tradeRepository = tradeRepository;
        _messageQueueService = messageQueueService;
        _logger = logger;
    }

    public async Task<Trade> ExecuteTradeAsync(TradeRequest request)
    {
        _logger.LogInformation("Executing trade for symbol: {Symbol}", request.Symbol);

        var trade = new Trade
        {
            Id = Guid.NewGuid(),
            Symbol = request.Symbol,
            Quantity = request.Quantity,
            Price = request.Price,
            ClientId = request.ClientId,
            Type = request.Type,
            ExecutionTime = DateTime.UtcNow,
            Status = TradeStatus.Pending
        };

        try
        {
            trade = await _tradeRepository.AddAsync(trade);

            trade.Status = TradeStatus.Executed;
            await _tradeRepository.UpdateAsync(trade);

            await _messageQueueService.PublishTradeAsync(trade);

            _logger.LogInformation($"Trade executed successfully. TradeId: {trade.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error executing trade for symbol: {request.Symbol}");
            trade.Status = TradeStatus.Failed;
            await _tradeRepository.UpdateAsync(trade);
            throw;
        }

        return trade;
    }

    public async Task<Trade> GetTradeByIdAsync(Guid id)
    {
        return await _tradeRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Trade>> GetTradesAsync(string clientId = null, DateTime? from = null, DateTime? to = null)
    {
        return await _tradeRepository.GetTradesAsync(clientId, from, to);
    }
}
