using Business.Services.Abstract;
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
    public class PlatformControllerTests
    {
        private readonly Mock<IPlatformService> _mockPlatformService;
        private readonly PlatformsController _controller;

        public PlatformControllerTests()
        {
            _mockPlatformService = new Mock<IPlatformService>();
            _controller = new PlatformsController(_mockPlatformService.Object);
        }

        [Fact]
        public async Task GetAllPlatforms_ReturnsOkResult_WhenPlatformsExist()
        {
            // Arrange
            var platforms = WebTestHelper.CreatePlatformResponseList(1);
            _mockPlatformService.Setup(service => service.GetAllPlatformsAsync()).ReturnsAsync(platforms).Verifiable();

            // Act
            var result = await _controller.GetAllPlatforms();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PlatformResponse>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task CreatePlatform_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var platformCreate = WebTestHelper.CreatePlatformCreateRequest();
            _mockPlatformService.Setup(service => service.CreatePlatformAsync(It.IsAny<string>()))
                .ReturnsAsync(Guid.NewGuid()).Verifiable();

            // Act
            var result = await _controller.CreatePlatform(platformCreate);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal("GetPlatformById", createdResult.ActionName);
        }

        [Fact]
        public async Task GetPlatformById_ReturnsOkResult_WhenPlatformExists()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            var platform = new PlatformResponse
            {
                Id = platformId,
                Type = "Test Platform"
            };

            _mockPlatformService
                .Setup(service => service.GetPlatformByIdAsync(platformId))
                .ReturnsAsync(platform)
                .Verifiable();

            // Act
            var result = await _controller.GetPlatformById(platformId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPlatform = Assert.IsType<PlatformResponse>(okResult.Value); 
            Assert.Equal(platformId, returnedPlatform.Id);
            Assert.Equal("Test Platform", returnedPlatform.Type);
            _mockPlatformService.Verify(); 
        }

        [Fact]
        public async Task GetPlatformById_ReturnsNotFound_WhenPlatformDoesNotExist()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            _mockPlatformService
                .Setup(service => service.GetPlatformByIdAsync(platformId))!
                .ReturnsAsync((PlatformResponse?)null)
                .Verifiable();

            // Act
            var result = await _controller.GetPlatformById(platformId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _mockPlatformService.Verify();
        }

        [Fact]
        public async Task UpdatePlatform_ReturnsNoContent_WhenUpdatedSuccessfully()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            var platformUpdate = new PlatformCreate
            { 
                Platform = new PlatformRequest
                {
                    Type = "new"
                }
            };
            _mockPlatformService.Setup(service => service.UpdatePlatformAsync(platformId, It.IsAny<string>())).ReturnsAsync(true).Verifiable();

            // Act
            var result = await _controller.UpdatePlatform(platformId, platformUpdate);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdatePlatform_ReturnsNotFound_WhenPlatformDoesNotExist()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            var platformUpdate = new PlatformCreate
            {
                Platform = new PlatformRequest
                {
                    Type = "new"
                }
            };
            _mockPlatformService.Setup(service => service.UpdatePlatformAsync(platformId, It.IsAny<string>())).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _controller.UpdatePlatform(platformId, platformUpdate);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeletePlatform_ReturnsNoContent_WhenDeletedSuccessfully()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            _mockPlatformService.Setup(service => service.DeletePlatformAsync(platformId)).ReturnsAsync(true).Verifiable();

            // Act
            var result = await _controller.DeletePlatform(platformId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeletePlatform_ReturnsNotFound_WhenPlatformDoesNotExist()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            _mockPlatformService.Setup(service => service.DeletePlatformAsync(platformId)).ReturnsAsync(false).Verifiable();

            // Act
            var result = await _controller.DeletePlatform(platformId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPlatformsByGameKey_ReturnsOkResult_WhenPlatformsExist()
        {
            // Arrange
            var gameKey = "gamekey";
            var platforms = WebTestHelper.CreatePlatformResponseList(1);
            _mockPlatformService.Setup(service => service.GetPlatformsByGameKeyAsync(gameKey)).ReturnsAsync(platforms).Verifiable();

            // Act
            var result = await _controller.GetPlatformsByGameKey(gameKey);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<PlatformResponse>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetPlatformsByGameKey_ReturnsNotFound_WhenPlatformsDoNotExist()
        {
            // Arrange
            var gameKey = "nonexistentkey";
            _mockPlatformService.Setup(service => service.GetPlatformsByGameKeyAsync(gameKey)).ReturnsAsync(new List<PlatformResponse>()).Verifiable();

            // Act
            var result = await _controller.GetPlatformsByGameKey(gameKey);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
