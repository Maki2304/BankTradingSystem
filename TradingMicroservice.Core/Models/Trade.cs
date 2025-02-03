using TradingMicroservice.Core.Enums;

namespace TradingMicroservice.Core.Models;

public class Trade
{
    public Guid Id { get; set; }
    public string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public string ClientId { get; set; }
    public TradeType Type { get; set; }
    public DateTime ExecutionTime { get; set; }
    public TradeStatus Status { get; set; }
    public decimal TotalAmount => Quantity * Price;
}
