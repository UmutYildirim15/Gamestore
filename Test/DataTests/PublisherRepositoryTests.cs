using Data;
using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;
using Xunit;

namespace Test.DataTests
{
    public class PublisherRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly PublisherRepository _repository;

        public PublisherRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new PublisherRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPublishers()
        {
            var publishers = DataTestsHelper.CreateRandomPublishers(3);
            DataTestsHelper.CreateMockDbSetForPublishers(publishers, _contextMock);

            var result = await _repository.GetAllAsync();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPublisher_WhenExists()
        {
            var publisherId = Guid.NewGuid();
            var publisher = DataTestsHelper.CreatePublisher(id: publisherId, companyName: "Test Publisher");

            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher> { publisher }, _contextMock);

            var result = await _repository.GetByIdAsync(publisherId);

            Assert.NotNull(result);
            Assert.Equal(publisherId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenPublisherDoesNotExist()
        {
            var publisherId = Guid.NewGuid();
            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher>(), _contextMock);

            var result = await _repository.GetByIdAsync(publisherId);
            Assert.Null(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddPublisher()
        {
            var publisher = DataTestsHelper.CreatePublisher(companyName: "New Publisher");

            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher>(), _contextMock);

            await _repository.AddAsync(publisher);
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePublisher()
        {
            var publisher = DataTestsHelper.CreatePublisher(companyName: "Updated Publisher");

            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher>(), _contextMock);

            await _repository.UpdateAsync(publisher);
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePublisher()
        {
            var publisher = DataTestsHelper.CreatePublisher(companyName: "Publisher To Delete");

            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher>(), _contextMock);
            await _repository.DeleteAsync(publisher);
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetByNameAsync_ShouldReturnPublisher_WhenExists()
        {
            var companyName = "Unique Publisher";
            var publisher = DataTestsHelper.CreatePublisher(companyName: companyName);

            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher> { publisher }, _contextMock);

            var result = await _repository.GetByNameAsync(companyName);
            Assert.NotNull(result);
            Assert.Equal(companyName, result.CompanyName);
        }

        [Fact]
        public async Task SearchPublishersAsync_ShouldReturnMatchingPublishers()
        {
            var publisher1 = DataTestsHelper.CreatePublisher(companyName: "Alpha Publisher");
            var publisher2 = DataTestsHelper.CreatePublisher(companyName: "Beta Publisher");
            var publisher3 = DataTestsHelper.CreatePublisher(companyName: "Gamma");
            
            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher> { publisher1, publisher2, publisher3 }, _contextMock);
            
            var result = await _repository.SearchPublishersAsync("Publisher");
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task IsNameUniqueAsync_ShouldReturnTrue_WhenNameUnique()
        {
            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher>(), _contextMock);
            var isUnique = await _repository.IsNameUniqueAsync("Unique Name");
            Assert.True(isUnique);
        }

        [Fact]
        public async Task IsNameUniqueAsync_ShouldReturnFalse_WhenNameExists()
        {
            var publisher = DataTestsHelper.CreatePublisher(companyName: "Existing Name");
            DataTestsHelper.CreateMockDbSetForPublishers(new List<Publisher> { publisher }, _contextMock);
            var isUnique = await _repository.IsNameUniqueAsync("Existing Name");
            Assert.False(isUnique);
        }
    }
}
