using Data;
using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;

namespace Test.DataTests
{
    public class GenreRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly GenreRepository _repository;

        public GenreRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new GenreRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllGenres()
        {
            var genres = DataTestsHelper.CreateRandomGenres(2);

            DataTestsHelper.CreateMockDbSetForGenres(genres, _contextMock);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnGenre_WhenGenreExists()
        {
            var genre = DataTestsHelper.CreateGenre();

            DataTestsHelper.CreateMockDbSetForGenres(new List<Genre> { genre }, _contextMock);

            var result = await _repository.GetByIdAsync(genre.Id);

            Assert.NotNull(result);
            Assert.Equal(genre.Id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenGenreDoesNotExist()
        {
            var genreId = Guid.NewGuid();

            DataTestsHelper.CreateMockDbSetForGenres(new List<Genre>(), _contextMock);

            var result = await _repository.GetByIdAsync(genreId);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnGenres_WhenIdsExist()
        {
            var genres = DataTestsHelper.CreateRandomGenres(2);

            DataTestsHelper.CreateMockDbSetForGenres(genres, _contextMock);

            if (genres != null)
            {
                var result = await _repository.GetByIdsAsync(new List<Guid> { genres[0].Id, genres[1].Id });

                var enumerable = result.ToList();
                Assert.Equal(2, enumerable.Count);
                Assert.Contains(enumerable, g => g.Id == genres[0].Id);
                Assert.Contains(enumerable, g => g.Id == genres[1].Id);
            }
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnEmpty_WhenNoGenresExist()
        {
            var genreIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            DataTestsHelper.CreateMockDbSetForGenres(new List<Genre>(), _contextMock);

            var result = await _repository.GetByIdsAsync(genreIds);

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddGenre()
        {
            var genre = DataTestsHelper.CreateGenre();

            DataTestsHelper.CreateMockDbSetForGenres(new List<Genre>(), _contextMock);

            await _repository.AddAsync(genre);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateGenre()
        {
            var genre = DataTestsHelper.CreateGenre();

            DataTestsHelper.CreateMockDbSetForGenres(new List<Genre>(), _contextMock);

            await _repository.UpdateAsync(genre);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteGenre()
        {
            var genre = DataTestsHelper.CreateGenre();

            DataTestsHelper.CreateMockDbSetForGenres(new List<Genre> { genre }, _contextMock);

            await _repository.DeleteAsync(genre.Id);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotDelete_WhenGenreDoesNotExist()
        {
            var genreId = Guid.NewGuid();

            DataTestsHelper.CreateMockDbSetForGenres(new List<Genre>(), _contextMock);

            await _repository.DeleteAsync(genreId);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}
