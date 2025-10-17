using Business.Facades;
using Business.Services.Abstract;
using Business.Services.Concrete;
using Business.Services.Dto;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Moq;
using Test.Helpers;

namespace Test.BusinessTests;

public class GameServiceTests
{
   private readonly Mock<IGameRepository> _mockGameRepository;
    private readonly Mock<IPlatformRepository> _mockPlatformRepository;
    private readonly Mock<IGenreRepository> _mockGenreRepository;
    private readonly Mock<IGamePlatformRepository> _mockGamePlatformRepository;
    private readonly Mock<IGameGenreRepository> _mockGameGenreRepository;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        (_mockGameRepository,
         _mockPlatformRepository,
         _mockGenreRepository,
         _mockGamePlatformRepository,
         _mockGameGenreRepository,
         var mockPublisherRepository,
         var mockOrderRepository,
         _) = BusinessTestsHelper.SetupMocksForGame<
                                IGameRepository,
                                IPlatformRepository,
                                IGenreRepository,
                                IGamePlatformRepository,
                                IGameGenreRepository,
                                IPublisherRepository,
                                IOrderRepository,
                                ILogger<IGameService>>();

        var mockLogger = new Mock<ILogger<IGameService>>();

        var repositoryFacade = new GameRepositoryFacade(
            _mockGameRepository.Object,
            _mockPlatformRepository.Object,
            _mockGenreRepository.Object,
            _mockGamePlatformRepository.Object,
            _mockGameGenreRepository.Object,
            mockPublisherRepository.Object,
            mockOrderRepository.Object
        );

        _gameService = new GameService(repositoryFacade, mockLogger.Object);
    }

    [Fact]
    public async Task GetAllGamesAsync_ReturnsGameResponses()
    {
        // Arrange
        var gameList = BusinessTestsHelper.ArrangeGames(_mockGameRepository);
        _mockGameRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(gameList).Verifiable();

        // Act
        var result = await _gameService.GetAllGamesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.Count());

    }

    [Fact]
    public async Task CreateGameAsync_CreatesGameSuccessfully()
    {
        // Arrange
        BusinessTestsHelper.ArrangeCreateGame(_mockGameRepository);
        var dto = new GameCreateDto
        {
            Name = "name"
        };
        _mockGameRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
        
        
        // Act
        var result = await _gameService.CreateGameAsync(dto);

        // Assert
        _mockGameRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        BusinessTestsHelper.AssertGuidIsNotEmpty(result);
        
    }

    [Fact]
    public async Task GetGameByIdAsync_ReturnsGameResponse()
    {
        // Arrange
        var game = BusinessTestsHelper.CreateGame(id:new Guid());
        _mockGameRepository.Setup(r => r.GetByIdAsync(game.Id)).ReturnsAsync(game).Verifiable();

        // Act
        var result = await _gameService.GetGameByIdAsync(game.Id);

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetGameByIdAsync_ThrowsKeyNotFoundException_WhenGameNotFound()
    {
        // Arrange
        var id1 = new Guid();
        var id2 = new Guid();
        
        var game = BusinessTestsHelper.CreateGame(id1);
        _mockGameRepository.Setup(r => r.GetByIdAsync(id2)).ReturnsAsync(game).Verifiable();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.GetGameByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateGameAsync_UpdatesGameSuccessfully()
    {
        // Arrange
        var genreIds = new List<Guid> { Guid.NewGuid() };
        var platformIds = new List<Guid> { Guid.NewGuid() };
        var oldGame = BusinessTestsHelper.CreateGame(id: new Guid());
        var dto = new GameCreateDto
        {
            Name = "name"
        };

        _mockGenreRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Genre> { new Genre { Id = genreIds.First(), Name = "Action" } });
        _mockPlatformRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(new List<Platform> { new Platform { Id = platformIds.First(), Type = "PC" } });
        _mockGameRepository.Setup(r => r.GetByIdAsync(oldGame.Id)).ReturnsAsync(oldGame);
        _mockGameRepository.Setup(r => r.UpdateAsync(It.IsAny<Game>())).Returns(Task.CompletedTask);
        
        _mockGameRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        // Act
        var result = await _gameService.UpdateGameAsync(oldGame.Id, dto);

        // Assert
        _mockGameRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result);
        
    }

    [Fact]
    public async Task DeleteGameAsync_DeletesGameSuccessfully()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Name = "Game",
            Description = "Description",
            Key = "gamekey",
            Price = 0,
            UnitInStock = 0,
            Discount = 0,
            PublisherId = Guid.NewGuid()
        };
        _mockGameRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);
        _mockGameGenreRepository.Setup(r => r.RemoveGameGenresAsync(It.IsAny<IEnumerable<GameGenre>>())).Returns(Task.CompletedTask);
        _mockGamePlatformRepository.Setup(r => r.RemoveGamePlatformsAsync(It.IsAny<IEnumerable<GamePlatform>>())).Returns(Task.CompletedTask);
        _mockGameRepository.Setup(r => r.DeleteAsync(It.IsAny<Game>())).Returns(Task.CompletedTask);
        
        _mockGameRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        // Act
        var result = await _gameService.DeleteGameAsync(gameId);

        // Assert
        _mockGameRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result);
        

    }

    [Fact]
    public async Task GetGameByKeyAsync_ReturnsGameResponse()
    {
        // Arrange
        var game = BusinessTestsHelper.CreateGame(id:new Guid(), key: "Test");
        _mockGameRepository.Setup(r => r.GetByKeyAsync(game.Key))
            .ReturnsAsync(game).Verifiable();
        
        // Act
        var result = await _gameService.GetGameByKeyAsync(game.Key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(game.Key, result.Key);
    }

    [Fact]
    public async Task GetGameByKeyAsync_ThrowsKeyNotFoundException_WhenGameNotFound()
    {
        // Arrange
        var game = BusinessTestsHelper.CreateGame(id:new Guid(), key: "nullkey");
        _mockGameRepository.Setup(r => r.GetByKeyAsync(game.Key)).Verifiable();

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _gameService.GetGameByKeyAsync("invalidkey"));
    }

    [Fact]
    public async Task DeleteGameByKeyAsync_DeletesGameSuccessfully()
    {
        // Arrange
        var game = BusinessTestsHelper.CreateGame(id: new Guid(), key: "Test");
        _mockGameRepository.Setup(r => r.GetByKeyAsync(game.Key)).ReturnsAsync(game);
        _mockGameRepository.Setup(r => r.DeleteAsync(It.IsAny<Game>())).Returns(Task.CompletedTask);
        
        _mockGameRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        // Act
        var result = await _gameService.DeleteGameByKeyAsync(game.Key);

        // Assert
        _mockGameRepository.Verify(r => r.DeleteAsync(It.IsAny<Game>()), Times.Once);
        _mockGameRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        Assert.True(result);
        
    }

    [Fact]
    public async Task GetGamesByPlatformAsync_ReturnsGameResponses()
    {
        // Arrange
        var game = BusinessTestsHelper.CreateGame(id:new Guid());
        if (game.Platforms != null)
        {
            _mockGameRepository.Setup(r => r.GetGamesByPlatformAsync(game.Platforms[0].PlatformId));
        }
        // Act
        if (game.Platforms != null)
        {
            var result = await _gameService.GetGamesByPlatformAsync(game.Platforms[0].PlatformId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }

    [Fact]
    public async Task GetGamesByGenreAsync_ReturnsGameResponses()
    {
        // Arrange
        var game = BusinessTestsHelper.CreateGame(id:new Guid());
        if (game.Genres != null)
        {
            _mockGameRepository.Setup(r => r.GetGamesByGenreAsync(game.Genres[0].GenreId));
        }
        // Act
        if (game.Genres != null)
        {
            var result = await _gameService.GetGamesByGenreAsync(game.Genres[0].GenreId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }
    }
}
