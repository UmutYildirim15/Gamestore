using Data;
using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;

namespace Test.DataTests
{
    public class GameGenreRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly GameGenreRepository _repository;

        public GameGenreRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new GameGenreRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetGameGenresByGameAsync_ShouldReturnGenres_WhenGameIdExists()
        {
            var gameId = Guid.NewGuid();
            var gameGenres = DataTestsHelper.CreateRandomGameGenres(2, gameId);

            DataTestsHelper.CreateMockDbSetForGameGenres(gameGenres, _contextMock);

            var result = await _repository.GetGameGenresByGameAsync(gameId);

            var enumerable = result.ToList();
            Assert.Equal(2, enumerable.Count);
            Assert.All(enumerable, gp => Assert.Equal(gameId, gp.GameId));
        }

        [Fact]
        public async Task GetGameGenresByGameAsync_ShouldReturnEmpty_WhenNoGameGenresFound()
        {
            var gameId = Guid.NewGuid();

            DataTestsHelper.CreateMockDbSetForGameGenres(new List<GameGenre>(), _contextMock);

            var result = await _repository.GetGameGenresByGameAsync(gameId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task RemoveGameGenresAsync_ShouldRemoveGenres()
        {
            var gameGenres = DataTestsHelper.CreateRandomGameGenres(2, Guid.NewGuid());

            DataTestsHelper.CreateMockDbSetForGameGenres(gameGenres, _contextMock);

            await _repository.RemoveGameGenresAsync(gameGenres);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}
