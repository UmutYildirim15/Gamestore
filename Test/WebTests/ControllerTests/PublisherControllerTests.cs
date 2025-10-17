using Business.Services.Abstract;
using Business.Services.Dto;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Dto;
using Web.Controllers;
using Web.Models.CreateModels;
using Web.Models.RequestModels;

namespace Test.WebTests.ControllerTests
{
    public class PublisherControllerTests
{
    private readonly Mock<IPublisherService> _mockPublisherService;
    private readonly PublisherController _controller;

    public PublisherControllerTests()
    {
        _mockPublisherService = new Mock<IPublisherService>();
        _controller = new PublisherController(_mockPublisherService.Object);
    }

    [Fact]
    public async Task GetGamesByPublisherAsync_ReturnsOk_WhenGamesExist()
    {
        // Arrange
        var companyName = "TestPublisher";
        var games = new List<GameResponse> { new GameResponse
            {
                Name = "Game1",
                Price = 0,
                UnitInStock = 0,
                Discount = 0
            }
        };
        _mockPublisherService.Setup(s => s.GetGamesByPublisherNameAsync(companyName))
            .ReturnsAsync(games);

        // Act
        var result = await _controller.GetGamesByPublisherAsync(companyName);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(games, okResult.Value);
    }

    [Fact]
    public async Task GetGamesByPublisherAsync_ReturnsNotFound_WhenNoGamesExist()
    {
        _mockPublisherService.Setup(s => s.GetGamesByPublisherNameAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<GameResponse>());

        var result = await _controller.GetGamesByPublisherAsync("NonExistentPublisher");
        
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdatePublisher_ReturnsNoContent_WhenUpdateIsSuccessful()
    {
        var request = new PublisherCreate { Publisher = new PublisherRequest { CompanyName = "Updated", HomePage = "https://example.com", Description = "Updated desc" } };
        _mockPublisherService.Setup(s => s.UpdatePublisherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        
        var result = await _controller.UpdatePublisher("ExistingPublisher", request);
        
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdatePublisher_ReturnsNotFound_WhenUpdateFails()
    {
        _mockPublisherService.Setup(s => s.UpdatePublisherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var request = new PublisherCreate { Publisher = new PublisherRequest { CompanyName = "Updated" } };
        var result = await _controller.UpdatePublisher("NonExistentPublisher", request);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllPublishers_ReturnsOk_WithListOfPublishers()
    {
        var publishers = new List<PublisherResponse> { new PublisherResponse { CompanyName = "Pub1" } };
        _mockPublisherService.Setup(s => s.GetAllPublishersAsync()).ReturnsAsync(publishers);

        var result = await _controller.GetAllPublishers();
        
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(publishers, okResult.Value);
    }

    [Fact]
    public async Task GetPublisherByName_ReturnsOk_WhenPublisherExists()
    {
        var publisher = new PublisherResponse { CompanyName = "Pub1" };
        _mockPublisherService.Setup(s => s.GetPublisherByNameAsync("Pub1")).ReturnsAsync(publisher);

        var result = await _controller.GetPublisherByName("Pub1");

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(publisher, okResult.Value);
    }

    [Fact]
    public async Task CreatePublisher_ReturnsCreatedAtAction()
    {
        var request = new PublisherCreate { Publisher = new PublisherRequest { CompanyName = "NewPublisher", HomePage = "https://example.com", Description = "New Description" } };
        var newPublisherId = Guid.NewGuid();
        _mockPublisherService.Setup(s => s.CreatePublisherAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(newPublisherId);

        var result = await _controller.CreatePublisher(request);
        
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal("GetPublisherByName", createdResult.ActionName);
    }

    [Fact]
    public async Task DeletePublisher_ReturnsNoContent()
    {
        _mockPublisherService.Setup(s => s.DeletePublisherAsync(It.IsAny<Guid>())).Returns(Task.CompletedTask);
        var result = await _controller.DeletePublisher(Guid.NewGuid());
        
        Assert.IsType<NoContentResult>(result);
    }
}

}