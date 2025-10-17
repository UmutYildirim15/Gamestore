using Data;
using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;

namespace Test.DataTests
{
    public class GameRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly GameRepository _repository;

        public GameRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new GameRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGames()
        {
            var games = DataTestsHelper.CreateRandomGames(2); 

            DataTestsHelper.CreateMockDbSetForGames(games, _contextMock);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGame_WhenGameExists()
        {
            var gameId = Guid.NewGuid();
            var game = DataTestsHelper.CreateGame(id: gameId, name: "Game 1", key: "game-key");

            DataTestsHelper.CreateMockDbSetForGames(new List<Game> { game }, _contextMock);

            var result = await _repository.GetByIdAsync(gameId);

            Assert.NotNull(result);
            Assert.Equal(gameId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenGameDoesNotExist()
        {
            var gameId = Guid.NewGuid();
            
            DataTestsHelper.CreateMockDbSetForGames(new List<Game>(), _contextMock);

            var result = await _repository.GetByIdAsync(gameId);
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddGame()
        {
            var game = DataTestsHelper.CreateGame(name: "New Game", key: "new-game-key");

            DataTestsHelper.CreateMockDbSetForGames(new List<Game>(), _contextMock);

            await _repository.AddAsync(game);
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateGame()
        {
            var game = DataTestsHelper.CreateGame(name: "Updated Game", key: "updated-game-key");

            DataTestsHelper.CreateMockDbSetForGames(new List<Game>(), _contextMock);

            await _repository.UpdateAsync(game);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteGame()
        {
            var game = DataTestsHelper.CreateGame(name: "Game to Delete", key: "delete-game-key");

            DataTestsHelper.CreateMockDbSetForGames(new List<Game>(), _contextMock);
            await _repository.DeleteAsync(game);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetByKeyAsync_ShouldReturnGame_WhenGameExists()
        {
            var gameKey = "game-key";
            var game = DataTestsHelper.CreateGame(key: gameKey, name: "Game 1");

            DataTestsHelper.CreateMockDbSetForGames(new List<Game> { game }, _contextMock);

            var result = await _repository.GetByKeyAsync(gameKey);

            Assert.NotNull(result);
            Assert.Equal(gameKey, result.Key);
        }

        [Fact]
        public async Task GetByKeyAsync_ShouldReturnNull_WhenGameDoesNotExist()
        {
            var gameKey = "game-key";

            DataTestsHelper.CreateMockDbSetForGames(new List<Game>(), _contextMock);

            var result = await _repository.GetByKeyAsync(gameKey);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetGamesByGenreAsync_ShouldReturnGames_WhenGenreExists()
        {
            var genreId = Guid.NewGuid();
            var game = DataTestsHelper.CreateGame(name: "Game 1", key: "game-1");
            var gameGenre = DataTestsHelper.CreateGameGenre(game.Id, genreId);

            DataTestsHelper.CreateMockDbSetForGameGenres(new List<GameGenre> { gameGenre }, _contextMock);

            var result = await _repository.GetGamesByGenreAsync(genreId);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetGamesByPlatformAsync_ShouldReturnGames_WhenPlatformExists()
        {
            var platformId = Guid.NewGuid();
            var game = DataTestsHelper.CreateGame(name: "Game 1", key: "game-1");
            var gamePlatform = DataTestsHelper.CreateGamePlatform(game.Id, platformId);

            DataTestsHelper.CreateMockDbSetForGamePlatforms(new List<GamePlatform> { gamePlatform }, _contextMock);
            

            var result = await _repository.GetGamesByPlatformAsync(platformId);

            Assert.Single(result);
        }

        [Fact]
        public async Task GetAllKeysStartingWithAsync_ShouldReturnKeys_WhenKeysExist()
        {
            var baseKey = "game-";
            var game = DataTestsHelper.CreateGame(key: "game-key", name: "Game 1");

            DataTestsHelper.CreateMockDbSetForGames(new List<Game> { game }, _contextMock);

            var result = await _repository.GetAllKeysStartingWithAsync(baseKey);

            Assert.Contains(result, k => k.StartsWith(baseKey));
        }
    }
}
