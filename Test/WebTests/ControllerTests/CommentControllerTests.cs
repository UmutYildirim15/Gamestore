using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Web.Controllers;
using Web.Models.RequestModels;

namespace Test.WebTests.ControllerTests
{
    public class CommentsControllerTests
    {
        private readonly CommentsController _controller;
        private readonly Mock<ICommentService> _mockCommentService;

        public CommentsControllerTests()
        {
            _mockCommentService = new Mock<ICommentService>();
            _controller = new CommentsController(_mockCommentService.Object);
        }

        [Fact]
        public async Task GetBanDurations_ReturnsOkResult_WithBanDurations()
        {
            // Arrange
            var expectedDurations = new List<string> { "1 hour", "1 day", "1 week", "1 month", "permanent" };
            _mockCommentService.Setup(x => x.GetBanDurationsAsync())
                .ReturnsAsync(expectedDurations);

            // Act
            var result = await _controller.GetBanDurations();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedDurations, okResult.Value);
        }

        [Fact]
        public async Task BanUser_ReturnsOkResult()
        {
            // Arrange
            var request = new BanUserRequest
            {
                User = "test",
                Duration = "1 month"
            };

            _mockCommentService.Setup(x => x.BanUserAsync(request.User, request.Duration))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.BanUser(request);

            // Assert
            Assert.IsType<OkResult>(result);
        }
    }
}