using TradingMicroservice.Core.Enums;

namespace TradingMicroservice.Core.Models;

public class TradeRequest
{
    public string Symbol { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public string ClientId { get; set; }
    public TradeType Type { get; set; }
}
