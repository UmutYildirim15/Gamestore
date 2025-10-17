using Data;
using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;

namespace Test.DataTests
{
    public class GamePlatformRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly GamePlatformRepository _repository;

        public GamePlatformRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new GamePlatformRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetGamePlatformsByGameAsync_ShouldReturnGamePlatforms_WhenGameExists()
        {
            var gameId = Guid.NewGuid();
            var gamePlatforms = DataTestsHelper.CreateRandomGamePlatforms(2, gameId);

            DataTestsHelper.CreateMockDbSetForGamePlatforms(gamePlatforms, _contextMock);

            var result = await _repository.GetGamePlatformsByGameAsync(gameId);

            var enumerable = result.ToList();
            Assert.Equal(2, enumerable.Count);
            Assert.All(enumerable, gp => Assert.Equal(gameId, gp.GameId));
        }

        [Fact]
        public async Task GetGamePlatformsByGameAsync_ShouldReturnEmpty_WhenNoGamePlatformsFound()
        {
            var gameId = Guid.NewGuid();

            DataTestsHelper.CreateMockDbSetForGamePlatforms(new List<GamePlatform>(), _contextMock);

            var result = await _repository.GetGamePlatformsByGameAsync(gameId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task RemoveGamePlatformsAsync_ShouldRemoveGamePlatforms()
        {
            var gamePlatforms = DataTestsHelper.CreateRandomGamePlatforms(2, new Guid());

            DataTestsHelper.CreateMockDbSetForGamePlatforms(gamePlatforms, _contextMock);

            await _repository.RemoveGamePlatformsAsync(gamePlatforms);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}