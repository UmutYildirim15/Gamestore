using Data;
using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;
using Xunit;

namespace Test.DataTests
{
    public class OrderRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly OrderRepository _repository;

        public OrderRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new OrderRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllOrders()
        {
            var orders = DataTestsHelper.CreateRandomOrders(3);
            DataTestsHelper.CreateMockDbSetForOrders(orders, _contextMock);

            var result = await _repository.GetAllAsync();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenOrderExists()
        {
            var orderId = Guid.NewGuid();
            var order = DataTestsHelper.CreateOrder(id: orderId);
            DataTestsHelper.CreateMockDbSetForOrders(new List<Order> { order }, _contextMock);

            var result = await _repository.GetByIdAsync(orderId);
            Assert.NotNull(result);
            Assert.Equal(orderId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var orderId = Guid.NewGuid();
            DataTestsHelper.CreateMockDbSetForOrders(new List<Order>(), _contextMock);
            var result = await _repository.GetByIdAsync(orderId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCartByCustomerIdAsync_ShouldReturnCart_WhenOpenCartExists()
        {
            var customerId = Guid.NewGuid();
            var cart = DataTestsHelper.CreateCart(customerId);
            DataTestsHelper.CreateMockDbSetForOrders(new List<Order> { cart }, _contextMock);

            var result = await _repository.GetCartByCustomerIdAsync(customerId);
            Assert.NotNull(result);
            Assert.Equal(OrderStatus.Open, result.Status);
        }

        [Fact]
        public async Task AddAsync_ShouldAddOrder()
        {
            var order = DataTestsHelper.CreateOrder();
            DataTestsHelper.CreateMockDbSetForOrders(new List<Order>(), _contextMock);

            await _repository.AddAsync(order);
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateOrder()
        {
            var order = DataTestsHelper.CreateOrder();
            DataTestsHelper.CreateMockDbSetForOrders(new List<Order>(), _contextMock);

            await _repository.UpdateAsync(order);
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteOrder()
        {
            var order = DataTestsHelper.CreateOrder();
            DataTestsHelper.CreateMockDbSetForOrders(new List<Order>(), _contextMock);
            await _repository.DeleteAsync(order);
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetOrdersByStatusAsync_ShouldReturnOrders_WithSpecifiedStatus()
        {
            var paidOrder = DataTestsHelper.CreateOrder(status: OrderStatus.Paid);
            var cancelledOrder = DataTestsHelper.CreateOrder(status: OrderStatus.Cancelled);
            var openOrder = DataTestsHelper.CreateOrder(status: OrderStatus.Open);
            DataTestsHelper.CreateMockDbSetForOrders(new List<Order> { paidOrder, cancelledOrder, openOrder }, _contextMock);

            var result = await _repository.GetOrdersByStatusAsync(new List<OrderStatus> { OrderStatus.Paid, OrderStatus.Cancelled });
            Assert.Equal(2, result.Count());
        }
    }
}
