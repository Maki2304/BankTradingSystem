using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Core.Enums;
using TradingMicroservice.Core.Models;
using TradingMicroservice.Infrastructure.Services;

namespace TradingMicroservice.UnitTests;

public class TradingServiceTests
{
    private readonly Mock<ITradeRepository> _mockTradeRepository;
    private readonly Mock<IMessageQueueService> _mockMessageQueueService;
    private readonly Mock<ILogger<TradingService>> _mockLogger;
    private readonly TradingService _tradingService;

    public TradingServiceTests()
    {
        _mockTradeRepository = new Mock<ITradeRepository>();
        _mockMessageQueueService = new Mock<IMessageQueueService>();
        _mockLogger = new Mock<ILogger<TradingService>>();

        _tradingService = new TradingService(
            _mockTradeRepository.Object,
            _mockMessageQueueService.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task ExecuteTradeAsync_ValidRequest_ShouldCreateTradeAndPublishMessage()
    {
        // Arrange
        var request = new TradeRequest
        {
            Symbol = "AAPL",
            Quantity = 100,
            Price = 150.00m,
            ClientId = "client123",
            Type = TradeType.Buy
        };

        var createdTrade = new Trade
        {
            Id = Guid.NewGuid(),
            Symbol = request.Symbol,
            Quantity = request.Quantity,
            Price = request.Price,
            ClientId = request.ClientId,
            Type = request.Type,
            ExecutionTime = DateTime.UtcNow,
            Status = TradeStatus.Executed
        };

        _mockTradeRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Trade>()))
            .ReturnsAsync(createdTrade);

        // Act
        var result = await _tradingService.ExecuteTradeAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Symbol.Should().Be(request.Symbol);
        result.Quantity.Should().Be(request.Quantity);
        result.Price.Should().Be(request.Price);
        result.Status.Should().Be(TradeStatus.Executed);

        _mockTradeRepository.Verify(
            repo => repo.AddAsync(It.IsAny<Trade>()),
            Times.Once
        );

        _mockMessageQueueService.Verify(
            mq => mq.PublishTradeAsync(It.IsAny<Trade>()),
            Times.Once
        );
    }

    [Fact]
    public async Task GetTradesAsync_ShouldReturnTradesFromRepository()
    {
        // Arrange
        var expectedTrades = new List<Trade>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Symbol = "AAPL",
                    Quantity = 100,
                    Price = 150.00m,
                    ClientId = "client123",
                    Type = TradeType.Buy,
                    ExecutionTime = DateTime.UtcNow,
                    Status = TradeStatus.Executed
                }
            };

        _mockTradeRepository
            .Setup(repo => repo.GetTradesAsync(null, null, null))
            .ReturnsAsync(expectedTrades);

        // Act
        var result = await _tradingService.GetTradesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Symbol.Should().Be("AAPL");
    }

    [Fact]
    public async Task GetTradeByIdAsync_ExistingTrade_ShouldReturnTrade()
    {
        // Arrange
        var tradeId = Guid.NewGuid();
        var expectedTrade = new Trade
        {
            Id = tradeId,
            Symbol = "AAPL",
            Quantity = 100,
            Price = 150.00m,
            ClientId = "client123",
            Type = TradeType.Buy,
            ExecutionTime = DateTime.UtcNow,
            Status = TradeStatus.Executed
        };

        _mockTradeRepository
            .Setup(repo => repo.GetByIdAsync(tradeId))
            .ReturnsAsync(expectedTrade);

        // Act
        var result = await _tradingService.GetTradeByIdAsync(tradeId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(tradeId);
        result.Symbol.Should().Be("AAPL");
    }

    [Fact]
    public async Task ExecuteTradeAsync_WhenRepositoryFails_ShouldThrowAndNotPublishMessage()
    {
        // Arrange
        var request = new TradeRequest
        {
            Symbol = "AAPL",
            Quantity = 100,
            Price = 150.00m,
            ClientId = "client123",
            Type = TradeType.Buy
        };

        _mockTradeRepository
            .Setup(repo => repo.AddAsync(It.IsAny<Trade>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _tradingService.ExecuteTradeAsync(request));

        _mockMessageQueueService.Verify(
            mq => mq.PublishTradeAsync(It.IsAny<Trade>()),
            Times.Never
        );
    }
}
