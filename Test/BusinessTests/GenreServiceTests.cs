using Business.Services.Abstract;
using Business.Services.Concrete;
using Data.Entities;
using Data.Repositories.Abstract;
using Moq;
using Test.Helpers;
using Microsoft.Extensions.Logging;

namespace Test.BusinessTests;

public class GenreServiceTests
{
    private readonly Mock<IGenreRepository> _mockGenreRepository;
    private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IGameGenreRepository> _mockGameGenreRepository;
    private readonly GenreService? _genreService;

    public GenreServiceTests(Mock<ILogger<IGenreService>> mockLogger)
    {
        Mock<ILogger<IGenreService>> mockLogger1 = mockLogger;
        (_mockGenreRepository, _mockGameRepository, _mockGameGenreRepository) =
            BusinessTestsHelper.SetupMocksForGenre<IGenreRepository, IGameRepository, IGameGenreRepository>();

        _genreService = new GenreService(
            _mockGenreRepository.Object,
            _mockGameRepository.Object,
            _mockGameGenreRepository.Object,
            mockLogger1.Object
        );
    }

    [Fact]
    public async Task GetAllGenresAsync_ReturnsGenreResponses()
    {
        // Arrange
        var genres = BusinessTestsHelper.ArrangeGenres(_mockGenreRepository, "Action", "Adventure");
        _mockGenreRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

        // Act
        if (_genreService != null)
        {
            var result = await _genreService.GetAllGenresAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }

    [Fact]
    public async Task CreateGenreAsync_CreatesGenreSuccessfully()
    {
        // Arrange
        var genre = BusinessTestsHelper.CreateGenre();
        _mockGenreRepository.Setup(r => r.AddAsync(genre)).Returns(Task.CompletedTask);
        _mockGenreRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
        // Act
        if (_genreService != null)
        {
            var result = await _genreService.CreateGenreAsync("New Genre", "parent genre name");

            // Assert
            _mockGenreRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.NotEqual(Guid.Empty, result);
        }
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsGenreResponse()
    {
        // Arrange
        var genre = BusinessTestsHelper.CreateGenre();
        _mockGenreRepository.Setup(r => r.GetByIdAsync(genre.Id)).ReturnsAsync(genre);

        // Act
        if (_genreService != null)
        {
            var result = await _genreService.GetGenreByIdAsync(genre.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(genre.Id, result.Id);
        }
    }

    [Fact]
    public async Task GetGenreByIdAsync_ReturnsNull_WhenGenreNotFound()
    {
        // Arrange
        BusinessTestsHelper.CreateGenre();
        _mockGenreRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Genre?)null);
        // Act
        if (_genreService != null)
        {
            var result = await _genreService.GetGenreByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task UpdateGenreAsync_UpdatesGenreSuccessfully()
    {
        // Arrange
        var genre = BusinessTestsHelper.CreateGenre(id:Guid.NewGuid());
        BusinessTestsHelper.ArrangeUpdateGenre(_mockGenreRepository, genre.Id, "Old Genre");
        _mockGenreRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        // Act
        var result = _genreService != null && await _genreService.UpdateGenreAsync(genre.Id, "Updated Genre");

        // Assert
        _mockGenreRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result);
        Assert.Equal("Updated Genre", genre.Name);
    }

    [Fact]
    public async Task UpdateGenreAsync_ReturnsFalse_WhenGenreNotFound()
    {
        // Arrange
        BusinessTestsHelper.ArrangeGenreNotFound(_mockGenreRepository);
        _mockGenreRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
        // Act
        var result = _genreService != null && await _genreService.UpdateGenreAsync(Guid.NewGuid(), "Updated Genre");

        // Assert
        _mockGenreRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteGenreAsync_DeletesGenreSuccessfully()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        BusinessTestsHelper.ArrangeDeleteGenre(_mockGenreRepository, genreId, "Action");
        _mockGenreRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        // Act
        var result = _genreService != null && await _genreService.DeleteGenreAsync(genreId);

        // Assert
        _mockGenreRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteGenreAsync_ReturnsFalse_WhenGenreNotFound()
    {
        // Arrange
        BusinessTestsHelper.ArrangeGenreNotFound(_mockGenreRepository);
        _mockGenreRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        // Act
        var result = _genreService != null && await _genreService.DeleteGenreAsync(Guid.NewGuid());

        // Assert
        _mockGenreRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
        Assert.False(result);
        
    }

    [Fact]
    public async Task GetGenresByGameKeyAsync_ReturnsGenres()
    {
        // Arrange
        var gameKey = "game1";
        BusinessTestsHelper.ArrangeGameGenres(_mockGameRepository, _mockGameGenreRepository, gameKey, "Action", "Adventure");
        

        // Act
        if (_genreService != null)
        {
            var result = await _genreService.GetGenresByGameKeyAsync(gameKey);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
