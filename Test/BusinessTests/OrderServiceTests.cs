using Business.Services.Concrete;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.BusinessTests
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly OrderService _orderService;
        private readonly Guid _stubCustomerId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f69afa2");

        public OrderServiceTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _gameRepositoryMock = new Mock<IGameRepository>();
            var loggerMock = new Mock<ILogger<OrderService>>();

            _orderService = new OrderService(
                _orderRepositoryMock.Object,
                _gameRepositoryMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task AddGameToCartAsync_Should_Add_Game_When_Available()
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = "test-game",
                Name = "Test Game",
                Price = 50,
                UnitInStock = 10,
                Discount = 0,
                PublisherId = default
            };
            var order = new Order { Id = Guid.NewGuid(), CustomerId = _stubCustomerId, OrderGames = new List<OrderGame>() };

            _gameRepositoryMock.Setup(repo => repo.GetByKeyAsync("test-game")).ReturnsAsync(game);
            _orderRepositoryMock.Setup(repo => repo.GetCartByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.UpdateAsync(order)).Returns(Task.CompletedTask);

            var result = await _orderService.AddGameToCartAsync("test-game");
            Assert.True(result);
        }

        [Fact]
        public async Task RemoveGameFromCartAsync_Should_Remove_Game()
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = "test-game",
                Name = "Test Game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = default
            };
            var order = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = _stubCustomerId,
                OrderGames = new List<OrderGame> { new OrderGame { ProductId = game.Id, Quantity = 1 } }
            };

            _gameRepositoryMock.Setup(repo => repo.GetByKeyAsync("test-game")).ReturnsAsync(game);
            _orderRepositoryMock.Setup(repo => repo.GetCartByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);
            _orderRepositoryMock.Setup(repo => repo.UpdateAsync(order)).Returns(Task.CompletedTask);

            var result = await _orderService.RemoveGameFromCartAsync("test-game");
            Assert.True(result);
        }

        [Fact]
        public async Task GetOrdersAsync_Should_Return_Orders()
        {
            var orders = new List<Order> { new Order { Id = Guid.NewGuid(), CustomerId = _stubCustomerId } };
            _orderRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(orders);

            var result = await _orderService.GetOrdersAsync();
            Assert.NotEmpty(result);
        }

        [Fact]
        public async Task GetOrderByIdAsync_Should_Return_Order()
        {
            var order = new Order { Id = Guid.NewGuid(), CustomerId = _stubCustomerId };
            _orderRepositoryMock.Setup(repo => repo.GetByIdAsync(order.Id)).ReturnsAsync(order);

            var result = await _orderService.GetOrderByIdAsync(order.Id);
            Assert.NotNull(result);
            Assert.Equal(order.Id, result.Id);
        }

        [Fact]
        public async Task GetCartAsync_Should_Return_Cart()
        {
            var order = new Order { Id = Guid.NewGuid(), CustomerId = _stubCustomerId, OrderGames = new List<OrderGame>() };
            _orderRepositoryMock.Setup(repo => repo.GetCartByCustomerIdAsync(_stubCustomerId)).ReturnsAsync(order);

            var result = await _orderService.GetCartAsync();
            Assert.NotNull(result);
        }
    }
}