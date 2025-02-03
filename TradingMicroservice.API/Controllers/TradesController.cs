using Microsoft.AspNetCore.Mvc;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Core.Models;

namespace TradingMicroservice.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TradesController : ControllerBase
{
    private readonly ITradingService _tradingService;
    private readonly ILogger<TradesController> _logger;

    public TradesController(ITradingService tradingService, ILogger<TradesController> logger)
    {
        _tradingService = tradingService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<Trade>> ExecuteTrade(TradeRequest request)
    {
        try
        {
            var trade = await _tradingService.ExecuteTradeAsync(request);
            return Ok(trade);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing trade");
            return StatusCode(500, new { message = "An error occurred while processing the trade" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Trade>> GetTrade(Guid id)
    {
        var trade = await _tradingService.GetTradeByIdAsync(id);
        if (trade == null)
        {
            return NotFound();
        }

        return Ok(trade);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Trade>>> GetTrades(
        [FromQuery] string? clientId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var trades = await _tradingService.GetTradesAsync(clientId, from, to);
        return Ok(trades);
    }
}
