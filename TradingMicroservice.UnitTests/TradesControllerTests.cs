using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TradingMicroservice.API.Controllers;
using TradingMicroservice.Core.Abstraction;
using TradingMicroservice.Core.Enums;
using TradingMicroservice.Core.Models;

namespace TradingMicroservice.UnitTests
{
    public class TradesControllerTests
    {
        private readonly Mock<ITradingService> _mockTradingService;
        private readonly Mock<ILogger<TradesController>> _mockLogger;
        private readonly TradesController _controller;

        public TradesControllerTests()
        {
            _mockTradingService = new Mock<ITradingService>();
            _mockLogger = new Mock<ILogger<TradesController>>();
            _controller = new TradesController(_mockTradingService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ExecuteTrade_ValidRequest_ReturnsOkResult()
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

            var expectedTrade = new Trade
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

            _mockTradingService
                .Setup(service => service.ExecuteTradeAsync(request))
                .ReturnsAsync(expectedTrade);

            // Act
            var result = await _controller.ExecuteTrade(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Trade>(okResult.Value);
            returnValue.Symbol.Should().Be(request.Symbol);
            returnValue.Quantity.Should().Be(request.Quantity);
            returnValue.Price.Should().Be(request.Price);
        }

        [Fact]
        public async Task GetTrade_ExistingId_ReturnsOkResult()
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

            _mockTradingService
                .Setup(service => service.GetTradeByIdAsync(tradeId))
                .ReturnsAsync(expectedTrade);

            // Act
            var result = await _controller.GetTrade(tradeId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Trade>(okResult.Value);
            returnValue.Id.Should().Be(tradeId);
        }

        [Fact]
        public async Task GetTrade_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var tradeId = Guid.NewGuid();
            _mockTradingService
                .Setup(service => service.GetTradeByIdAsync(tradeId))
                .ReturnsAsync((Trade)null);

            // Act
            var result = await _controller.GetTrade(tradeId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetTrades_ReturnsAllTrades()
        {
            // Arrange
            var expectedTrades = new List<Trade>
            {
                new Trade
                {
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

            _mockTradingService
                .Setup(service => service.GetTradesAsync(null, null, null))
                .ReturnsAsync(expectedTrades);

            // Act
            var result = await _controller.GetTrades();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Trade>>(okResult.Value);
            returnValue.Should().HaveCount(1);
        }

        [Fact]
        public async Task ExecuteTrade_WhenServiceThrows_ReturnsInternalServerError()
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

            _mockTradingService
                .Setup(service => service.ExecuteTradeAsync(request))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.ExecuteTrade(request);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            statusCodeResult.StatusCode.Should().Be(500);
        }
    }
}
