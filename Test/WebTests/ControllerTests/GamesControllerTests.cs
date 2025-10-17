using Business.Services.Abstract;
using Business.Services.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Dto;
using Test.Helpers;
using Web.Controllers;
using Web.Models.CreateModels;
using Web.Models.RequestModels;

namespace Test.WebTests.ControllerTests
{
    public class GamesControllerTests
    {
        private readonly Mock<IGameService> _mockGameService;
        private readonly GamesController _controller;

        public GamesControllerTests()
        {
            _mockGameService = new Mock<IGameService>();
            Mock<ICommentService> mockCommentService = new();
            _controller = new GamesController(_mockGameService.Object, mockCommentService.Object);
        }

        [Fact]
        public async Task GetAllGames_SetsTotalCountHeader()
        {
            // Arrange
            var games = WebTestHelper.CreateGameResponseList(2);
            _mockGameService.Setup(service => service.GetAllGamesAsync())
                .ReturnsAsync(games)
                .Verifiable();

            var httpContext = new DefaultHttpContext();
            httpContext.Response.Headers["x-total-numbers-of-games"] = string.Empty;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            GameQuery query = new();
            await _controller.GetAllGames(query);

            // Assert
            Assert.Equal("2", _controller.HttpContext.Response.Headers["x-total-numbers-of-games"]);
        }

        [Fact]
        public async Task CreateGame_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var gameCreate = WebTestHelper.CreateGameCreateRequest(publisherId: Guid.NewGuid());
            _mockGameService.Setup(service =>
                    service.CreateGameAsync(It.IsAny<GameCreateDto>()))
                .ReturnsAsync(Guid.NewGuid())
                .Verifiable();

            // Act
            var result = await _controller.CreateGame(gameCreate);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetGameById", createdResult.ActionName);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task GetGameById_ReturnsOkResult_WhenGameExists()
        {
            // Arrange
            var game = WebTestHelper.CreateSingleGameResponse();
            _mockGameService.Setup(service => service.GetGameByIdAsync(game.Id))
                .ReturnsAsync(game)
                .Verifiable();

            // Act
            var result = await _controller.GetGameById(game.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<GameResponse>(okResult.Value);
            Assert.Equal(game.Id, returnValue.Id);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task GetGameById_ReturnsNotFoundResult_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _mockGameService.Setup(service => service.GetGameByIdAsync(gameId))
                .ReturnsAsync((GameResponse?)null)
                .Verifiable();

            // Act
            var result = await _controller.GetGameById(gameId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task UpdateGame_ReturnsNoContent_WhenUpdatedSuccessfully()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameUpdate = WebTestHelper.CreateGameCreateRequest(
                publisherId: Guid.NewGuid(),
                key: "updatedKey",
                name: "Updated Game",
                description: "Updated Description");

            _mockGameService.Setup(service =>
                    service.UpdateGameAsync(gameId, It.IsAny<GameCreateDto>()))
                .ReturnsAsync(true)
                .Verifiable();

            // Act
            var result = await _controller.UpdateGame(gameId, gameUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task UpdateGame_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var gameUpdate = WebTestHelper.CreateGameCreateRequest(
                publisherId: Guid.NewGuid(),
                key: "updatedKey",
                name: "Updated Game",
                description: "Updated Description");

            _mockGameService.Setup(service =>
                    service.UpdateGameAsync(gameId, It.IsAny<GameCreateDto>()))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            var result = await _controller.UpdateGame(gameId, gameUpdate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task DeleteGame_ReturnsNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _mockGameService.Setup(service => service.DeleteGameAsync(gameId))
                .ReturnsAsync(true)
                .Verifiable();

            // Act
            var result = await _controller.DeleteGame(gameId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task DeleteGame_ReturnsNotFound_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _mockGameService.Setup(service => service.DeleteGameAsync(gameId))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            var result = await _controller.DeleteGame(gameId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task GetPublisherByGameKey_ReturnsOk_WhenPublisherExists()
        {
            // Arrange
            var gameKey = "existing-game-key";
            var publisher = new PublisherResponse
            {
                CompanyName = "Test Publisher",
                HomePage = "https://test.com",
                Description = "Test Description"
            };

            _mockGameService.Setup(service => service.GetPublisherByGameKeyAsync(gameKey))
                .ReturnsAsync(publisher)
                .Verifiable();

            // Act
            var result = await _controller.GetPublisherByGameKey(gameKey);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PublisherResponse>(okResult.Value);
            Assert.Equal(publisher.CompanyName, returnValue.CompanyName);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task GetPublisherByGameKey_ReturnsNotFound_WhenPublisherDoesNotExist()
        {
            // Arrange
            var gameKey = "non-existing-game-key";
            _mockGameService.Setup(service => service.GetPublisherByGameKeyAsync(gameKey))
                .ReturnsAsync((PublisherResponse?)null)
                .Verifiable();

            // Act
            var result = await _controller.GetPublisherByGameKey(gameKey);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task BuyGame_ReturnsOk_WhenPurchaseSucceeds()
        {
            // Arrange
            var gameKey = "valid-game-key";
            var purchaseRequest = new PurchaseRequest { UserId = Guid.NewGuid() };

            _mockGameService.Setup(service => service.BuyGameAsync(gameKey, purchaseRequest.UserId))
                .ReturnsAsync(true)
                .Verifiable();

            // Act
            var result = await _controller.BuyGame(gameKey, purchaseRequest);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockGameService.Verify();
        }

        [Fact]
        public async Task BuyGame_ReturnsBadRequest_WhenPurchaseFails()
        {
            // Arrange
            var gameKey = "invalid-game-key";
            var purchaseRequest = new PurchaseRequest { UserId = Guid.NewGuid() };

            _mockGameService.Setup(service => service.BuyGameAsync(gameKey, purchaseRequest.UserId))
                .ReturnsAsync(false)
                .Verifiable();

            // Act
            var result = await _controller.BuyGame(gameKey, purchaseRequest);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Could not process the purchase.", badRequestResult.Value);
            _mockGameService.Verify();
        }
    }
}
