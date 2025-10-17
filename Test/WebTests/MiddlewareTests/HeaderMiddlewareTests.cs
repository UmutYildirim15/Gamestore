using Business.Services.Abstract;
using Business.Services.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shared.Dto;
using Web.Middleware;

namespace Test.WebTests.MiddlewareTests
{
    public class HeaderMiddlewareTests
    {
        [Fact]
        public async Task HeaderMiddleware_Should_Add_Header_When_Cache_Empty()
        {
            // Arrange
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            new ServiceCollection()
                .AddSingleton<IMemoryCache>(memoryCache)
                .BuildServiceProvider();

            var scopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            var gameServiceMock = new Mock<IGameService>();
            gameServiceMock.Setup(x => x.GetAllGamesAsync())
                .ReturnsAsync(new List<GameResponse> { new GameResponse
                {
                    Name = "Name",
                    Price = 0,
                    UnitInStock = 0,
                    Discount = 0
                } });

            serviceProviderMock.Setup(x => x.GetService(typeof(IGameService)))
                .Returns(gameServiceMock.Object);

            serviceScopeMock.Setup(x => x.ServiceProvider)
                .Returns(serviceProviderMock.Object);
            scopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);

            var middleware = new HeaderMiddleware(_ => Task.CompletedTask, scopeFactoryMock.Object, memoryCache);
            var context = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            Assert.True(context.Response.Headers.ContainsKey("x-total-numbers-of-games"));
            Assert.Equal("2", context.Response.Headers["x-total-numbers-of-games"]);
        }
    }
}

