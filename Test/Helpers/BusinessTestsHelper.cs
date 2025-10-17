using Data.Entities;
using Data.Repositories.Abstract;
using Moq;

namespace Test.Helpers
{
    public static class BusinessTestsHelper
    {
        public static (Mock<T1>, Mock<T2>, Mock<T3>) SetupMocksForGenre<T1, T2, T3>()
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return (new Mock<T1>(), new Mock<T2>(), new Mock<T3>());
        }
        public static (Mock<T1>, Mock<T2>, Mock<T3>, Mock<T4>, Mock<T5>, Mock<T6>, Mock<T7>, Mock<T8>) SetupMocksForGame<T1, T2, T3, T4, T5, T6, T7, T8>()
            where T1 : class
            where T2 : class
            where T3 : class
            where T4 : class
            where T5 : class
            where T6 : class
            where T7 : class
            where T8 : class
        {
            return (new Mock<T1>(), new Mock<T2>(), new Mock<T3>(), new Mock<T4>(), new Mock<T5>(), new Mock<T6>(), new Mock<T7>(), new Mock<T8>());
        }
        public static (Mock<T1>, Mock<T2>, Mock<T3>) SetupMocksForPlatform<T1, T2, T3>()
            where T1 : class
            where T2 : class
            where T3 : class
        {
            return (new Mock<T1>(), new Mock<T2>(), new Mock<T3>());
        }
        public static List<Game> ArrangeGames(Mock<IGameRepository> mockRepo, int count = 5)
        {
            var random = new Random();
            return Enumerable.Range(1, count).Select(_ => new Game
            {
                Id = Guid.NewGuid(),
                Name = $"Game-{random.Next(1000, 9999)}",
                Key = $"key-{Guid.NewGuid().ToString("N").Substring(0, 8)}",
                Description = $"Description-{random.Next(1000, 9999)}",
                Price = random.Next(1000, 9999),
                UnitInStock = random.Next(1000, 9999),
                Discount = random.Next(1000, 9999),
                PublisherId = Guid.NewGuid()
            }).ToList();
        }
        public static List<Genre> ArrangeGenres(Mock<IGenreRepository> mockRepo, params string[] genreNames)
        {
            return genreNames.Select(name => CreateGenre(name: name)).ToList();
        }
        
        public static List<Platform> ArrangePlatforms(Mock<IPlatformRepository> mockRepo, params string[] platformNames)
        {
            return platformNames.Select(type => CreatePlatform(type: type)).ToList();
        }

        public static void ArrangeGenreNotFound(Mock<IGenreRepository> mockRepo)
        {
            mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(null as Genre);
        }

        public static void ArrangeUpdateGenre(Mock<IGenreRepository> mockRepo, Guid id, string? oldName)
        {
            var genre = CreateGenre(id, oldName);
            mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(genre);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Genre>())).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        }

        public static void ArrangeDeleteGenre(Mock<IGenreRepository> mockRepo, Guid id, string? name)
        {
            var genre = CreateGenre(id, name);
            mockRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(genre);
            mockRepo.Setup(r => r.DeleteAsync(id)).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        }

        public static void ArrangeGameGenres(
            Mock<IGameRepository> mockGameRepo,
            Mock<IGameGenreRepository> mockGameGenreRepo,
            string gameKey,
            params string[] genreNames)
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = gameKey,
                Name = "Default Game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = Guid.NewGuid()
            };
            var gameGenres = genreNames.Select(name =>
                new GameGenre
                {
                    GameId = game.Id,
                    GenreId = Guid.NewGuid(),
                    Genre = CreateGenre(name: name)
                }).ToList();

            mockGameRepo.Setup(r => r.GetByKeyAsync(gameKey)).ReturnsAsync(game);
            mockGameGenreRepo.Setup(r => r.GetGameGenresByGameAsync(game.Id)).ReturnsAsync(gameGenres);
            mockGameRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            mockGameGenreRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        }

        public static Genre CreateGenre(Guid? id = null, string? name = null)
        {
            return new Genre { Id = id ?? Guid.NewGuid(), Name = name ?? "Default Genre" };
        }
        
        public static Platform CreatePlatform(Guid? id = null, string? type = null)
        {
            return new Platform { Id = id ?? Guid.NewGuid(), Type = type ?? "Default Platform" };
        }
        
        public static Game CreateGame(Guid id, string? key = "Default Key", string? name = "Default Name")
        {
            return new Game
            {
                Id = id,
                Name = name ?? "Default Game",
                Key = key ?? "default-key",
                Description = "Default Description",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = Guid.NewGuid()
            };
            
        }
        public static void AssertGuidIsNotEmpty(Guid id)
        {
            Assert.NotEqual(Guid.Empty, id);
        }
        public static void ArrangeCreateGame(Mock<IGameRepository> mockRepo)
        {
            mockRepo.Setup(r => r.AddAsync(It.IsAny<Game>())).Returns(Task.CompletedTask);
            mockRepo.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable();

        }

        public static List<GamePlatform> ArrangeGamePlatforms(Mock<IGamePlatformRepository> mockRepo, Guid gameId, params string[] platformTypes)
        {
            return platformTypes.Select(type => new GamePlatform
            {
                GameId = gameId,
                PlatformId = Guid.NewGuid(),
                Platform = new Platform { Type = type }
            }).ToList();

        }
    }
}