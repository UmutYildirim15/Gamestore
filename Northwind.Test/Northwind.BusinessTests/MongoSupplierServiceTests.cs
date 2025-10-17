using System.Linq.Expressions;
using AutoMapper;
using Business.Services.Dto;
using Moq;
using Northwind.Business.Services.Concrete;
using Northwind.Data.Entities;
using Northwind.Data.Logger;
using Northwind.Data.Repositories.Abstract;
using Shared.Dto;
using Xunit;

namespace Northwind.Test.Northwind.BusinessTests;

public class MongoSupplierServiceTests
{
    private const string Culture = "tr";
    private readonly Mock<IMongoUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MongoSupplierService _service;

    public MongoSupplierServiceTests()
    {
        _mockUnitOfWork = new Mock<IMongoUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        Mock<IEntityLogger> mockLogger = new();
        _service = new MongoSupplierService(_mockUnitOfWork.Object, _mockMapper.Object, mockLogger.Object);
    }

    [Fact]
    public async Task GetPublisherByIdAsync_ReturnsPublisherResponse()
    {
        // Arrange
        var supplier = new Supplier
        {
            CompanyName = "test"
        };
        _mockUnitOfWork.Setup(u => u.Suppliers.GetOneAsync(It.IsAny<Expression<Func<Supplier, bool>>>()))
            .ReturnsAsync(supplier);

        var expected = new PublisherResponse
        {
            CompanyName = "test"
        };
        _mockMapper.Setup(m => m.Map<PublisherResponse>(supplier)).Returns(expected);

        // Act
        var result = await _service.GetPublisherByIdAsync("some-id", Culture);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetAllPublishersAsync_ReturnsPublisherResponseList()
    {
        // Arrange
        var suppliers = new List<Supplier> { new()
            {
                CompanyName = "test"
            }
        };
        _mockUnitOfWork.Setup(u => u.Suppliers.GetAllAsync(It.IsAny<Expression<Func<Supplier, bool>>>()))
            .ReturnsAsync(suppliers);

        var expected = new List<PublisherResponse> { new()
            {
                CompanyName = "test"
            }
        };
        _mockMapper.Setup(m => m.Map<IList<PublisherResponse>>(suppliers)).Returns(expected);

        // Act
        var result = await _service.GetAllPublishersAsync(Culture);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetGamesByPublisherIdAsync_ReturnsGameResponseList()
    {
        // Arrange
        const string id = "some-id";
        var products = new List<Product> { new()
            {
                ProductName = "test",
                Key = "test"
            }
        };
        _mockUnitOfWork.Setup(u => u.Suppliers.GetProductsBySupplierIdAsync(id)).ReturnsAsync(products);

        var expected = new List<GameResponse> { new()
            {
                Name = "test",
                Price = 0,
                UnitInStock = 0,
                Discount = 0
            }
        };
        _mockMapper.Setup(m => m.Map<IList<GameResponse>>(products)).Returns(expected);

        // Act
        var result = await _service.GetGamesByPublisherIdAsync(id, Culture);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task DeletePublisherAsync_RemovesPublisherAndSavesChanges()
    {
        // Arrange
        const string id = "1";
        _mockUnitOfWork.Setup(u => u.Suppliers.DeleteAsync(id))
            .Verifiable();
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .Verifiable();

        // Act
        await _service.DeletePublishersAsync(id);

        // Assert
        _mockUnitOfWork.Verify(u => u.Suppliers.DeleteAsync(id), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}