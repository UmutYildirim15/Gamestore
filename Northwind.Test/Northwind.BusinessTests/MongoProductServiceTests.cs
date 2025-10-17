using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using AutoMapper;
using Business.Services.Dto;
using Moq;
using Northwind.Business.Services.Concrete;
using Northwind.Data.Entities;
using Northwind.Data.Logger;
using Northwind.Data.Repositories.Abstract;
using Shared.Dto;
using Shared.Models;
using Xunit;

namespace Northwind.Test.Northwind.BusinessTests;

public class MongoProductServiceTests
{
    private const string DefaultKey = "Key";
    private readonly Mock<IMongoUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MongoProductService _service;

    public MongoProductServiceTests()
    {
        _mockUnitOfWork = new Mock<IMongoUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        Mock<IEntityLogger> mockLogger = new();
        _service = new MongoProductService(_mockUnitOfWork.Object, _mockMapper.Object, mockLogger.Object);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ReturnsGameResponse()
    {
        // Arrange
        var product = new Product
        {
            Key = DefaultKey,
            ProductName = "test"
        };
        _mockUnitOfWork.Setup(u => u.Products.GetOneAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(product);

        var expected = new GameResponse
        {
            Key = DefaultKey,
            Name = "test",
            Price = 0,
            UnitInStock = 0,
            Discount = 0
        };
        _mockMapper.Setup(m => m.Map<GameResponse>(product)).Returns(expected);

        // Act
        var result = await _service.GetGameByKeyAsync(DefaultKey, string.Empty);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetGameByIdAsync_ReturnsGameResponse()
    {
        // Arrange
        const string id = "1";
        Guid id2 = new Guid(id);
        var product = new Product
        {
            ProductId = long.Parse(id),
            ProductName = "test",
            Key = "test"
        };
        _mockUnitOfWork.Setup(u => u.Products.GetOneAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(product);

        var expected = new GameResponse
        {
            Id = id2,
            Name = "test",
            Price = 0,
            UnitInStock = 0,
            Discount = 0
        };
        _mockMapper.Setup(m => m.Map<GameResponse>(product)).Returns(expected);

        // Act
        var result = await _service.GetGameByIdAsync(id, string.Empty);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetGenresByGameKeyAsync_ReturnsGenreResponses()
    {
        // Arrange
        var categories = new List<Category> { new()
        {
            CategoryName = "test"
        }, new()
            {
                CategoryName = "test"
            }
        };
        _mockUnitOfWork.Setup(u => u.Products.GetCategoriesByProductKeyAsync(DefaultKey))
            .ReturnsAsync(categories);

        var expected = new List<GenreResponse> { new()
            {
                Name = "test"
            }, new()
            {
                Name = "test"
            }
        };
        _mockMapper.Setup(m => m.Map<IList<GenreResponse>>(categories)).Returns(expected);

        // Act
        var result = await _service.GetGenresByGameKeyAsync(DefaultKey);

        // Assert
        Assert.Equal(expected, result);
        Assert.Equal(expected.Count, result.Count);
    }

    [Fact]
    public async Task GetPlatformsByGameKeyAsync_ReturnsEmptyPlatformBriefDtos()
    {
        const string key = "Key";
        var result = await _service.GetPlatformsByGameKeyAsync(key);

        Assert.Empty(result);
    }

    [Fact]
    public async Task GetPublisherByGameKeyAsync_ReturnsPublisherResponse()
    {
        // Arrange
        var supplier = new Supplier
        {
            CompanyName = "test"
        };
        _mockUnitOfWork.Setup(u => u.Products.GetSupplierByProductKeyAsync(DefaultKey))
            .ReturnsAsync(supplier);

        var expected = new PublisherResponse
        {
            CompanyName = "test"
        };
        _mockMapper.Setup(m => m.Map<PublisherResponse>(supplier)).Returns(expected);

        // Act
        var result = await _service.GetPublisherByGameKeyAsync(DefaultKey);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task GetAllGamesAsync_SupportedFilter_CallsRepository()
    {
        // Arrange
        var filter = new GameFilter();
        var filteringResult = new EntityFilter<Product>(new List<Product>(), 2);

        _mockUnitOfWork.Setup(u => u.Products.GetFilteredProductsAsync(filter))
            .ReturnsAsync(filteringResult);

        var records = new List<GameResponse>();
        _mockMapper.Setup(m => m.Map<IList<GameResponse>>(filteringResult.Records))
            .Returns(records);

        // Act
        var result = await _service.GetFilteredGamesAsync(filter);

        // Assert
        Assert.Equal(2, result.TotalNoLimit);
        Assert.Equal(records, result.Records);
    }


    [Fact]
    public async Task GetAllGamesAsync_NotSupportedFilter_ReturnsEmpty()
    {
        // Arrange
        var filter = new GameFilter()
        {
            Platforms = new List<Guid>() { Guid.Empty },
            DatePublishing = DateTime.MinValue.ToString(CultureInfo.InvariantCulture),
        };

        // Act
        var result = await _service.GetFilteredGamesAsync(filter);

        // Assert
        Assert.Equal(0, result.TotalNoLimit);
        Assert.Empty(result.Records);
    }

    [Fact]
    public async Task DownloadAsync_ReturnsFileContentAndName()
    {
        // Arrange
        var product = new Product { Key = DefaultKey, ProductName = "name" };
        _mockUnitOfWork.Setup(u => u.Products.GetOneAsync(It.IsAny<Expression<Func<Product, bool>>>()))
            .ReturnsAsync(product);

        // Act
        (byte[] bytes, string fileName) = await _service.DownloadAsync(DefaultKey);

        // Assert
        var expectedContent = $"Game: {product.ProductName}\n\nDescription:";
        Assert.Equal(Encoding.UTF8.GetBytes(expectedContent), bytes);
        Assert.Contains(product.ProductName, fileName);
    }
}