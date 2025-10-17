using System.Linq.Expressions;
using Data;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;

namespace Test.Helpers
{
    public static class DataTestsHelper
    {
        public static Game CreateGame(Guid? id = null, string name = "Default Game", string key = "default-game-key")
        {
            return new Game
            {
                Id = id ?? Guid.NewGuid(),
                Name = name,
                Key = key,
                Genres = new List<GameGenre>(), 
                Platforms = new List<GamePlatform>(),
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = Guid.NewGuid()
            };
        }

        public static Genre CreateGenre(Guid? id = null, string name = "Default Genre")
        {
            return new Genre
            {
                Id = id ?? Guid.NewGuid(),
                Name = name
            };
        }
        
        public static Platform CreatePlatform(Guid? id = null, string type = "Default Platform")
        {
            return new Platform
            {
                Id = id ?? Guid.NewGuid(),
                Type = type
            };
        }
        
        public static GameGenre CreateGameGenre(Guid gameId, Guid genreId)
        {
            return new GameGenre
            {
                GameId = gameId,
                GenreId = genreId
            };
        }
        
        public static GamePlatform CreateGamePlatform(Guid gameId, Guid platformId)
        {
            return new GamePlatform
            {
                GameId = gameId,
                PlatformId = platformId
            };
        }
        
        public static Comment CreateComment(Guid id, string name, string body, Guid? parentId, Guid gameId)
        {
            return new Comment
            {
                Id = id,
                Name = name,
                Body = body,
                ParentCommentId = parentId,
                GameId = gameId
            };
        }

        public static Mock<DbSet<Comment>> CreateMockDbSetForComments(IEnumerable<Comment>? comments, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(comments);
            contextMock.Setup(c => c.Comments).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }
        
        public static Mock<DbSet<Publisher>> CreateMockDbSetForPublishers(List<Publisher>? publishers, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(publishers);
            contextMock.Setup(c => c.Publishers).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }
        
        public static Mock<DbSet<Order>> CreateMockDbSetForOrders(List<Order>? orders, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(orders);
            contextMock.Setup(c => c.Orders).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }

        public static Mock<DbSet<Game>> CreateMockDbSetForGames(List<Game>? games, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(games);
            contextMock.Setup(c => c.Games).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }

        public static Mock<DbSet<Genre>> CreateMockDbSetForGenres(List<Genre>? genres, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(genres);
            contextMock.Setup(c => c.Genres).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }

        public static Mock<DbSet<Platform>> CreateMockDbSetForPlatforms(List<Platform>? platforms, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(platforms);
            contextMock.Setup(c => c.Platforms).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }
        
        public static Mock<DbSet<GamePlatform>> CreateMockDbSetForGamePlatforms(List<GamePlatform>? gamePlatforms, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(gamePlatforms);
            contextMock.Setup(c => c.GamePlatforms).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }
        
        public static Mock<DbSet<GameGenre>> CreateMockDbSetForGameGenres(List<GameGenre>? gameGenres, Mock<ApplicationDbContext> contextMock)
        {
            var mockDbSet = CreateMockDbSet(gameGenres);
            contextMock.Setup(c => c.GameGenres).Returns(mockDbSet.Object).Verifiable();
            return mockDbSet;
        }

        private static Mock<DbSet<T>> CreateMockDbSet<T>(IEnumerable<T>? sourceList) where T : class
        {
            var queryable = (sourceList ?? throw new ArgumentNullException(nameof(sourceList))).AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(default))
                .Returns(new TestAsyncEnumerator<T>(queryable.GetEnumerator()));

            dbSetMock.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(queryable.Provider));

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression).Verifiable();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType).Verifiable();
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator).Verifiable();

            return dbSetMock;
        }

        public static List<Game>? CreateRandomGames(int count)
        {
            var games = new List<Game>();
            for (int i = 0; i < count; i++)
            {
                games.Add(new Game
                {
                    Id = Guid.NewGuid(),
                    Name = "Game " + i,
                    Key = "gamekey",
                    Price = 0,
                    UnitInStock = 0,
                    Discount = 0,
                    PublisherId = Guid.NewGuid()
                });
            }
            return games;
        }

        public static List<Genre>? CreateRandomGenres(int count)
        {
            var genres = new List<Genre>();
            for (int i = 0; i < count; i++)
            {
                genres.Add(new Genre { Id = Guid.NewGuid(), Name = "Genre " + i });
            }
            return genres;
        }
        
        public static List<Platform>? CreateRandomPlatforms(int count)
        {
            var platforms = new List<Platform>();
            for (int i = 0; i < count; i++)
            {
                platforms.Add(new Platform { Id = Guid.NewGuid(), Type = "Platform " + i });
            }
            return platforms;
        }
        
        public static List<GamePlatform>? CreateRandomGamePlatforms(int count, Guid gameId)
        {
            var gamePlatforms = new List<GamePlatform>();
            for (int i = 0; i < count; i++)
            {
                gamePlatforms.Add(new GamePlatform
                {
                    GameId = gameId,
                    PlatformId = Guid.NewGuid()
                });
            }
            return gamePlatforms;
        }
        
        public static List<GameGenre>? CreateRandomGameGenres(int count, Guid gameId)
        {
            var gameGenres = new List<GameGenre>();
            for (int i = 0; i < count; i++)
            {
                gameGenres.Add(new GameGenre
                {
                    GameId = gameId,
                    GenreId = Guid.NewGuid()
                });
            }
            return gameGenres;
        }
        
        public static List<Platform>? CreateMultiplePlatforms(Guid platformId1, Guid platformId2)
        {
            return new List<Platform>
            {
                new Platform { Id = platformId1, Type = "Platform 0" },
                new Platform { Id = platformId2, Type = "Platform 1" }
            };
        }
        public static Order CreateOrder(Guid? id = null, Guid? customerId = null, OrderStatus status = OrderStatus.Paid, DateTime? date = null)
        {
            return new Order
            {
                Id = id ?? Guid.NewGuid(),
                CustomerId = customerId ?? Guid.NewGuid(),
                Status = status,
                Date = date ?? DateTime.UtcNow,
                OrderGames = new List<OrderGame>()
            };
        }
        public static List<Publisher>? CreateRandomPublishers(int count)
        {
            var publishers = new List<Publisher>();
            for (int i = 0; i < count; i++)
            {
                publishers.Add(new Publisher
                {
                    Id = Guid.NewGuid(),
                    CompanyName = $"Publisher {i}"
                });
            }
            return publishers;
        }
        public static Publisher CreatePublisher(Guid? id = null, string companyName = "Default Publisher")
        {
            return new Publisher
            {
                Id = id ?? Guid.NewGuid(),
                CompanyName = companyName
            };
        }
        public static Order CreateCart(Guid customerId)
        {
            return new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                Status = OrderStatus.Open,
                Date = DateTime.UtcNow,
                OrderGames = new List<OrderGame>()
            };
        }
        public static List<Order>? CreateRandomOrders(int count)
        {
            var orders = new List<Order>();
            for (int i = 0; i < count; i++)
            {
                orders.Add(CreateOrder(status: OrderStatus.Paid));
            }
            return orders;
        }
    }
    
    public class TestAsyncEnumerator<T>(IEnumerator<T> enumerator) : IAsyncEnumerator<T>
    {
        public T Current => enumerator.Current;
        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(enumerator.MoveNext());
        public ValueTask DisposeAsync()
        {
            enumerator.Dispose();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }

    public class TestAsyncQueryProvider<TEntity>(IQueryProvider inner) : IAsyncQueryProvider
    {
        public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<TEntity>(expression);
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new TestAsyncEnumerable<TElement>(expression);
        public object? Execute(Expression expression) => inner.Execute(expression);
        public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);
        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = Execute(expression);
            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult })!;
        }
    }

    public class TestAsyncEnumerable<T>(Expression expression)
        : EnumerableQuery<T>(expression), IAsyncEnumerable<T>, IQueryable<T>
    {
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        IQueryProvider IQueryable.Provider => new TestAsyncQueryProvider<T>(this);
    }
}
