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

public class MongoCategoryServiceTests
{
    private readonly Mock<IMongoUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MongoCategoryService _service;

    public MongoCategoryServiceTests()
    {
        _mockUnitOfWork = new Mock<IMongoUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        Mock<IEntityLogger> mockLogger = new();
        _service = new MongoCategoryService(_mockUnitOfWork.Object, _mockMapper.Object, mockLogger.Object);
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsGenreFullDto()
    {
        const string id = "genreId";
        var genre = new Category
        {
            Id = id,
            CategoryName = "test"
        };
        _mockUnitOfWork.Setup(u => u.Categories.GetOneAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(genre);

        var expected = new GenreResponse
        {
            Id = new Guid(),
            Name = "test"
        };
        _mockMapper.Setup(m => m.Map<GenreResponse>(genre)).Returns(expected);

        var result = await _service.GetGenreByIdAsync(id, string.Empty);

        Assert.Equal(expected.Id, result.Id);
    }

    [Fact]
    public async Task GetAllGenresAsync_ReturnsGenreBriefDtoList()
    {
        var genres = new List<Category> { new()
        {
            Id = "id1",
            CategoryName = "test"
        }, new()
            {
                Id = "id2",
                CategoryName = "test"
            }
        };
        _mockUnitOfWork.Setup(u => u.Categories.GetAllAsync(null)).ReturnsAsync(genres);

        var expected = new List<GenreResponse> { new()
            {
                Id = new Guid(),
                Name = "test"
            }, new()
            {
                Id = new Guid(),
                Name = "test"
            }
        };
        _mockMapper.Setup(m => m.Map<IList<GenreResponse>>(genres)).Returns(expected);

        var result = await _service.GetAllGenresAsync(string.Empty);

        Assert.Equal(expected.Count, result.Count);
    }

    [Fact]
    public async Task GetSubgenresByParentAsync_ReturnsGenreBriefDtoList()
    {
        const string parentId = "parentId";
        Guid parentId1 = Guid.NewGuid();
        var genres = new List<Category> { new()
            {
                ParentId = parentId,
                CategoryName = "test"
            }
        };
        _mockUnitOfWork.Setup(u => u.Categories.GetAllAsync(It.IsAny<Expression<Func<Category, bool>>>()))
            .ReturnsAsync(genres);

        var expected = new List<GenreResponse> { new()
            {
                ParentGenreId = parentId1,
                Name = "test"
            }
        };
        _mockMapper.Setup(m => m.Map<IList<GenreResponse>>(genres)).Returns(expected);

        var result = await _service.GetSubgenresByParentAsync(parentId, string.Empty);

        Assert.Equal(expected.Count, result.Count);
    }

    [Fact]
    public async Task GetGamesByGenreIdAsync_ReturnsGameBriefDtoList()
    {
        const string genreId = "genreId";
        var games = new List<Product> { new()
        {
            CategoryId = 123,
            ProductName = "test",
            Key = "test"
        }};
        _mockUnitOfWork.Setup(u => u.Categories.GetProductsByCategoryIdAsync(genreId))
            .ReturnsAsync(games);

        var expected = new List<GameResponse> { new()
        {
            Name = "test",
            Key = null,
            Id = Guid.Empty,
            Description = null,
            Price = 0,
            UnitInStock = 0,
            Discount = 0
        }};
        _mockMapper.Setup(m => m.Map<IList<GameResponse>>(games))
            .Returns(expected);

        var result = await _service.GetGamesByGenreIdAsync(genreId, string.Empty);

        Assert.Equal(expected, result);
    }


    [Fact]
    public async Task DeleteGenreAsync_DeletesGenre()
    {
        const string genreId = "genreId";
        _mockUnitOfWork.Setup(u => u.Categories.DeleteAsync(genreId))
            .Verifiable();
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync())
            .Verifiable();

        await _service.DeleteGenreAsync(genreId);

        _mockUnitOfWork.Verify(u => u.Categories.DeleteAsync(genreId), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}