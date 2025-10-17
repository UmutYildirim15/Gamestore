using Business.Services.Abstract;
using Business.Services.Concrete;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Moq;
using Test.Helpers;

namespace Test.BusinessTests
{
    public class PlatformServiceTests
    {
        private readonly Mock<IPlatformRepository> _mockPlatformRepository;
        private readonly Mock<IGamePlatformRepository> _mockGamePlatformRepository;
        private readonly PlatformService _platformService;

        public PlatformServiceTests(Mock<ILogger<IPlatformService>>? mockLogger, PlatformService platformService)
        {
            Mock<ILogger<IPlatformService>>? mockLogger1 = mockLogger;
            _platformService = platformService;
            (_mockPlatformRepository, Mock<IGameRepository> mockGameRepository, _mockGamePlatformRepository) =
                BusinessTestsHelper.SetupMocksForPlatform<IPlatformRepository, IGameRepository, IGamePlatformRepository>();

            if (mockLogger1 != null)
                _platformService = new PlatformService(
                    _mockPlatformRepository.Object,
                    mockGameRepository.Object,
                    _mockGamePlatformRepository.Object,
                    mockLogger1.Object
                );
        }

        [Fact]
        public async Task CreatePlatformAsync_CreatesPlatformSuccessfully()
        {
            // Arrange
            var platform = BusinessTestsHelper.CreatePlatform(id: Guid.NewGuid());
            _mockPlatformRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _platformService.CreatePlatformAsync(platform.Type);

            // Assert
            _mockPlatformRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task UpdatePlatformAsync_UpdatesPlatformSuccessfully()
        {
            // Arrange
            var platform = BusinessTestsHelper.CreatePlatform(id:Guid.NewGuid());
            _mockPlatformRepository.Setup(r => r.GetByIdAsync(platform.Id)).ReturnsAsync(platform);
            _mockPlatformRepository.Setup(r => r.UpdateAsync(It.IsAny<Platform>())).Returns(Task.CompletedTask);
            _mockPlatformRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            
            // Act
            var result = await _platformService.UpdatePlatformAsync(platform.Id, "updatedType");

            // Assert
            _mockPlatformRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.True(result);
            Assert.Equal("updatedType", platform.Type);
        }

        [Fact]
        public async Task DeletePlatformAsync_DeletesPlatformSuccessfully()
        {
            // Arrange
            var platform = BusinessTestsHelper.CreatePlatform(id:Guid.NewGuid());
            _mockPlatformRepository.Setup(r => r.GetByIdAsync(platform.Id)).ReturnsAsync(platform);
            _mockPlatformRepository.Setup(r => r.DeleteAsync(platform.Id)).Returns(Task.CompletedTask);
            _mockPlatformRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();
            
            // Act
            var result = await _platformService.DeletePlatformAsync(platform.Id);

            // Assert
            _mockPlatformRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.True(result);
        }

        [Fact]
        public async Task GetAllPlatformsAsync_ReturnsPlatformResponses()
        {
            // Arrange
            var platforms = BusinessTestsHelper.ArrangePlatforms(_mockPlatformRepository, "PC", "Mobile");
            _mockPlatformRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(platforms);

            // Act
            var result = await _platformService.GetAllPlatformsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetPlatformByIdAsync_ReturnsPlatformResponse()
        {
            // Arrange
            var platform = BusinessTestsHelper.CreatePlatform(id: Guid.NewGuid());
            _mockPlatformRepository.Setup(r => r.GetByIdAsync(platform.Id)).ReturnsAsync(platform);

            // Act
            var result = await _platformService.GetPlatformByIdAsync(platform.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(platform.Id, result.Id);
        }

        [Fact]
        public async Task GetPlatformByIdAsync_ThrowsException_WhenPlatformNotFound()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            _mockPlatformRepository.Setup(r => r.GetByIdAsync(platformId)).ReturnsAsync((Platform?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _platformService.GetPlatformByIdAsync(platformId));
        }

        [Fact]
        public async Task UpdatePlatformAsync_ReturnsFalse_WhenPlatformNotFound()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            _mockPlatformRepository.Setup(r => r.GetByIdAsync(platformId)).ReturnsAsync((Platform?)null);
            _mockPlatformRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _platformService.UpdatePlatformAsync(platformId, "Updated Type");

            // Assert
            _mockPlatformRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            Assert.False(result);
        }

        [Fact]
        public async Task DeletePlatformAsync_ReturnsFalse_WhenPlatformNotFound()
        {
            // Arrange
            var platformId = Guid.NewGuid();
            _mockPlatformRepository.Setup(r => r.GetByIdAsync(platformId)).ReturnsAsync((Platform?)null);
            _mockPlatformRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _platformService.DeletePlatformAsync(platformId);

            // Assert
            _mockPlatformRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
            Assert.False(result);
            
        }

        [Fact]
        public async Task GetPlatformsByGameKeyAsync_ReturnsPlatforms()
        {
            // Arrange
            var game = BusinessTestsHelper.CreateGame(id:new Guid());
            var gamePlatforms = BusinessTestsHelper.ArrangeGamePlatforms(_mockGamePlatformRepository, game.Id, "PC", "Mobile");
            _mockGamePlatformRepository.Setup(r => r.GetGamePlatformsByGameAsync(game.Id)).ReturnsAsync(gamePlatforms);
            _mockGamePlatformRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

            // Act
            var result = await _platformService.GetPlatformsByGameKeyAsync(game.Key);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }
}
