using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Dto;
using Test.Helpers;
using Web.Controllers;

namespace Test.WebTests.ControllerTests
{
    public class GenresControllerTests
    {
        private readonly Mock<IGenreService> _mockGenreService;
        private readonly GenresController _controller;

        public GenresControllerTests()
        {
            _mockGenreService = new Mock<IGenreService>();
            _controller = new GenresController(_mockGenreService.Object);
        }

        [Fact]
        public async Task GetAllGenres_ReturnsOkResult_WhenGenresExist()
        {
            // Arrange
            var genres = WebTestHelper.CreateGenreResponseList(1);
            _mockGenreService.Setup(service => service.GetAllGenresAsync()).ReturnsAsync(genres).Verifiable();

            // Act
            var result = await _controller.GetAllGenres();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<GenreResponse>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task CreateGenre_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var genreCreate = WebTestHelper.CreateGenreCreateRequest();
            _mockGenreService.Setup(service => service.CreateGenreAsync(It.IsAny<string>(), null))
                .ReturnsAsync(Guid.NewGuid()).Verifiable();

            // Act
            var result = await _controller.CreateGenre(genreCreate);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetGenreById", createdResult.ActionName);
        }

        [Fact]
        public async Task GetGenreById_ReturnsOkResult_WhenGenreExists()
        {
            // Arrange
            var genre = WebTestHelper.CreateSingleGenreResponse();
            _mockGenreService.Setup(service => service.GetGenreByIdAsync(genre.Id)).ReturnsAsync(genre).Verifiable();

            // Act
            var result = await _controller.GetGenreById(genre.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GenreResponse>(okResult.Value);
            Assert.Equal(genre.Id, returnValue.Id);
        }

        [Fact]
        public async Task GetGenreById_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            // Arrange
            var genreId = Guid.NewGuid();
            _mockGenreService.Setup(service => service.GetGenreByIdAsync(genreId)).ReturnsAsync((GenreResponse?)null)
                .Verifiable();

            // Act
            var result = await _controller.GetGenreById(genreId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateGenre_ReturnsNoContent_WhenUpdatedSuccessfully()
        {
            // Arrange
            var genreId = Guid.NewGuid();
            var genreUpdate = WebTestHelper.CreateGenreUpdateRequest();
            _mockGenreService.Setup(service => service.UpdateGenreAsync(genreId, genreUpdate.Genre.Name)).ReturnsAsync(true).Verifiable();

            // Act
            var result = await _controller.UpdateGenre(genreId, genreUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateGenre_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            // Arrange
            var genreId = Guid.NewGuid();
            var genreUpdate = WebTestHelper.CreateGenreUpdateRequest();
            _mockGenreService.Setup(service => service.UpdateGenreAsync(genreId, genreUpdate.Genre.Name)).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _controller.UpdateGenre(genreId, genreUpdate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGenre_ReturnsNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            var genreId = Guid.NewGuid();
            _mockGenreService.Setup(service => service.DeleteGenreAsync(genreId)).ReturnsAsync(true).Verifiable();

            // Act
            var result = await _controller.DeleteGenre(genreId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteGenre_ReturnsNotFound_WhenGenreDoesNotExist()
        {
            // Arrange
            var genreId = Guid.NewGuid();
            _mockGenreService.Setup(service => service.DeleteGenreAsync(genreId)).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _controller.DeleteGenre(genreId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGenresByGameKey_ReturnsOkResult_WhenGenresExist()
        {
            // Arrange
            var gameKey = "gamekey";
            var genres = WebTestHelper.CreateGenreResponseList(1);
            _mockGenreService.Setup(service => service.GetGenresByGameKeyAsync(gameKey)).ReturnsAsync(genres).Verifiable();

            // Act
            var result = await _controller.GetGenresByGameKey(gameKey);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<GenreResponse>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetGenresByGameKey_ReturnsNotFound_WhenGenresDoNotExist()
        {
            // Arrange
            var gameKey = "nonexistentkey";
            _mockGenreService.Setup(service => service.GetGenresByGameKeyAsync(gameKey)).ReturnsAsync(new List<GenreResponse>()).Verifiable();

            // Act
            var result = await _controller.GetGenresByGameKey(gameKey);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
