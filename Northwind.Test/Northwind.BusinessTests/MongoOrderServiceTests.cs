using System.Linq.Expressions;
using AutoMapper;
using Moq;
using Northwind.Business.Services.Concrete;
using Northwind.Data.Entities;
using Northwind.Data.Logger;
using Northwind.Data.Repositories.Abstract;
using Shared.Dto;
using Shared.Models;
using Xunit;

namespace Northwind.Test.Northwind.BusinessTests;

public class MongoOrderServiceTests
{
    private readonly Mock<IMongoUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MongoOrderService _service;

    public MongoOrderServiceTests()
    {
        _mockUnitOfWork = new Mock<IMongoUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        Mock<IEntityLogger> mockLogger = new();
        _service = new MongoOrderService(_mockUnitOfWork.Object, _mockMapper.Object, mockLogger.Object);
    }

    [Fact]
    public async Task GetFilteredOrdersAsync_ReturnsOrderBriefDtoList()
    {
        // Arrange
        var orders = new List<Order> { new() };
        _mockUnitOfWork.Setup(u => u.Orders.GetFilteredOrdersAsync(It.IsAny<OrderFilter>()))
            .ReturnsAsync(orders);

        var expected = new List<OrderDetailResponse> { new() };
        _mockMapper.Setup(m => m.Map<IList<OrderDetailResponse>>(orders))
            .Returns(expected);

        // Act
        var result = await _service.GetFilteredOrdersAsync(new OrderFilter(DateTime.MinValue, DateTime.MaxValue));

        // Assert
        Assert.Equal(expected.Count, result.Count);
    }


    [Fact]
    public async Task GetOrderByIdAsync_ReturnsOrderBriefDto()
    {
        // Arrange
        const string orderId = "orderId";
        Guid orderId2 = new Guid();
        var order = new Order { Id = orderId };
        _mockUnitOfWork.Setup(u => u.Orders.GetOneAsync(It.IsAny<Expression<Func<Order, bool>>>())).ReturnsAsync(order);

        var expected = new OrderResponse { Id = orderId2 };
        _mockMapper.Setup(m => m.Map<OrderResponse>(order)).Returns(expected);

        // Act
        var result = await _service.GetOrderByIdAsync(orderId);

        // Assert
        Assert.Equal(expected.Id, result.Id);
    }

    [Fact]
    public async Task GetOrderDetailsAsync_ReturnsOrderDetailDtoList()
    {
        // Arrange
        const string orderId = "orderId";
        var details = new List<OrderDetail> { new() { OrderId = 1234 } };
        _mockUnitOfWork.Setup(u => u.OrderDetails.GetAllByOrderObjectIdAsync(orderId)).ReturnsAsync(details);

        var expected = new List<OrderDetailResponse> { new() };
        _mockMapper.Setup(m => m.Map<IList<OrderDetailResponse>>(details)).Returns(expected);

        // Act
        var result = await _service.GetOrderDetailsAsync(orderId);

        // Assert
        Assert.Equal(expected.Count, result.Count);
    }
}