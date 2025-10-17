using Data;
using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;

namespace Test.DataTests
{
    public class PlatformRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly PlatformRepository _repository;

        public PlatformRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new PlatformRepository(_contextMock.Object);
        }
        

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllPlatforms()
        {
            var platforms = DataTestsHelper.CreateRandomPlatforms(2); 

            DataTestsHelper.CreateMockDbSetForPlatforms(platforms, _contextMock);

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnPlatform_WhenPlatformExists()
        {
            var platformId = Guid.NewGuid();
            var platform = DataTestsHelper.CreatePlatform(id: platformId, type: "platform 1");

            DataTestsHelper.CreateMockDbSetForPlatforms(new List<Platform> { platform }, _contextMock);

            var result = await _repository.GetByIdAsync(platformId);

            Assert.NotNull(result);
            Assert.Equal(platformId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenPlatformDoesNotExist()
        {
            var platformId = Guid.NewGuid();
            
            DataTestsHelper.CreateMockDbSetForPlatforms(new List<Platform>(), _contextMock);

            var result = await _repository.GetByIdAsync(platformId);
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnPlatforms_WhenIdsExist()
        {
            var platformId1 = Guid.NewGuid();
            var platformId2 = Guid.NewGuid();
            var platforms= DataTestsHelper.CreateMultiplePlatforms(platformId1, platformId2);

            DataTestsHelper.CreateMockDbSetForPlatforms(platforms, _contextMock);

            var result = await _repository.GetByIdsAsync(new List<Guid> { platformId1, platformId2 });

            var enumerable = result.ToList();
            Assert.Equal(2, enumerable.Count);
            Assert.Contains(enumerable, p => p.Id == platformId1);
            Assert.Contains(enumerable, p => p.Id == platformId2);
        }

        [Fact]
        public async Task GetByIdsAsync_ShouldReturnEmpty_WhenNoPlatformsExist()
        {
            var platformIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            DataTestsHelper.CreateMockDbSetForPlatforms(new List<Platform>(), _contextMock);

            var result = await _repository.GetByIdsAsync(platformIds);

            Assert.Empty(result);
        }

        [Fact]
        public async Task AddAsync_ShouldAddPlatform()
        {
            var platform = DataTestsHelper.CreatePlatform();

            DataTestsHelper.CreateMockDbSetForPlatforms(new List<Platform>(), _contextMock);

            await _repository.AddAsync(platform);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePlatform()
        {
            var platform = DataTestsHelper.CreatePlatform();

            DataTestsHelper.CreateMockDbSetForPlatforms(new List<Platform>(), _contextMock);

            await _repository.UpdateAsync(platform);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeletePlatform()
        {
            var platformId = Guid.NewGuid();
            var platform = DataTestsHelper.CreatePlatform(platformId);

            DataTestsHelper.CreateMockDbSetForPlatforms(new List<Platform> { platform }, _contextMock);

            await _repository.DeleteAsync(platformId);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldNotDelete_WhenPlatformDoesNotExist()
        {
            var platformId = Guid.NewGuid();
            DataTestsHelper.CreateMockDbSetForPlatforms(new List<Platform>(), _contextMock);

            await _repository.DeleteAsync(platformId);

            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }
    }
}
