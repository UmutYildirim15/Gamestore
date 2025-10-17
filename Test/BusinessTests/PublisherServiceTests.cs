using Business.Services.Abstract;
using Business.Services.Concrete;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.BusinessTests
{
    public class PublisherServiceTests
    {
        private readonly Mock<IPublisherRepository> _publisherRepositoryMock = new();
        private readonly Mock<IGameService> _gameServiceMock = new();
        private readonly Mock<ILogger<IPublisherService>> _loggerMock = new();
        private readonly PublisherService _publisherService;

        public PublisherServiceTests()
        {
            _publisherService = new PublisherService(_publisherRepositoryMock.Object, _gameServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CreatePublisherAsync_ShouldCreatePublisher()
        {
            string companyName = "Test Company";
            string homePage = "https://test.com";
            string description = "Test Description";

            _publisherRepositoryMock.Setup(repo => repo.IsNameUniqueAsync(companyName)).ReturnsAsync(true);
            _publisherRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Publisher>())).Returns(Task.CompletedTask);

            var result = await _publisherService.CreatePublisherAsync(companyName, homePage, description);

            Assert.NotEqual(Guid.Empty, result);
            _publisherRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Publisher>()), Times.Once);
        }

        [Fact]
        public async Task GetPublisherByNameAsync_ShouldReturnPublisher()
        {
            string companyName = "Test Company";
            var publisher = new Publisher
            {
                Id = Guid.NewGuid(), CompanyName = companyName, HomePage = "https://test.com",
                Description = "Test Description"
            };
            _publisherRepositoryMock.Setup(repo => repo.GetByNameAsync(companyName)).ReturnsAsync(publisher);

            var result = await _publisherService.GetPublisherByNameAsync(companyName);

            Assert.NotNull(result);
            Assert.Equal(companyName, result.CompanyName);
        }

        [Fact]
        public async Task GetAllPublishersAsync_ShouldReturnPublishers()
        {
            var publishers = new List<Publisher>
            {
                new Publisher
                {
                    Id = Guid.NewGuid(), CompanyName = "Test1", HomePage = "https://test1.com",
                    Description = "Description1"
                },
                new Publisher
                {
                    Id = Guid.NewGuid(), CompanyName = "Test2", HomePage = "https://test2.com",
                    Description = "Description2"
                }
            };
            _publisherRepositoryMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(publishers);

            var result = await _publisherService.GetAllPublishersAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdatePublisherAsync_ShouldUpdatePublisher()
        {
            string oldName = "Old Company";
            string newName = "New Company";
            var publisher = new Publisher
                { Id = Guid.NewGuid(), CompanyName = oldName, HomePage = "https://old.com", Description = "Old Desc" };

            _publisherRepositoryMock.Setup(repo => repo.GetByNameAsync(oldName)).ReturnsAsync(publisher);
            _publisherRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Publisher>())).Returns(Task.CompletedTask);

            var result = await _publisherService.UpdatePublisherAsync(oldName, newName, "https://new.com", "New Desc");

            Assert.True(result);
        }

        [Fact]
        public async Task DeletePublisherAsync_ShouldDeletePublisherAndRelatedGames()
        {
            var publisherId = Guid.NewGuid();
            var publisher = new Publisher { Id = publisherId, CompanyName = "Test Publisher" };
            var games = new List<Game>
            {
                new Game
                {
                    Id = Guid.NewGuid(),
                    Name = "Test Game",
                    Key = "Test Key",
                    Price = 0,
                    UnitInStock = 0,
                    Discount = 0,
                    PublisherId = default
                }
            };

            _publisherRepositoryMock.Setup(repo => repo.GetByIdAsync(publisherId)).ReturnsAsync(publisher);
            _publisherRepositoryMock.Setup(repo => repo.GetGamesByPublisherIdAsync(publisherId)).ReturnsAsync(games);
            _publisherRepositoryMock.Setup(repo => repo.DeleteAsync(publisher)).Returns(Task.CompletedTask);
            _gameServiceMock.Setup(service => service.DeleteGameAsync(It.IsAny<Guid>())).Equals(Task.CompletedTask);

            await _publisherService.DeletePublisherAsync(publisherId);

            _gameServiceMock.Verify(service => service.DeleteGameAsync(It.IsAny<Guid>()), Times.Once);
            _publisherRepositoryMock.Verify(repo => repo.DeleteAsync(publisher), Times.Once);
        }

        [Fact]
        public async Task GetGamesByPublisherNameAsync_ShouldReturnGames()
        {
            string companyName = "Test Publisher";
            var publisher = new Publisher { Id = Guid.NewGuid(), CompanyName = companyName };
            var games = new List<Game>
            {
                new Game
                {
                    Id = Guid.NewGuid(),
                    Name = "Game 1",
                    Key = "Test Key",
                    Price = 0,
                    UnitInStock = 0,
                    Discount = 0,
                    PublisherId = default
                }
            };

            _publisherRepositoryMock.Setup(repo => repo.GetByNameAsync(companyName)).ReturnsAsync(publisher);
            _publisherRepositoryMock.Setup(repo => repo.GetGamesByPublisherIdAsync(publisher.Id)).ReturnsAsync(games);

            var result = await _publisherService.GetGamesByPublisherNameAsync(companyName);

            Assert.Single(result);
        }
    }
}

