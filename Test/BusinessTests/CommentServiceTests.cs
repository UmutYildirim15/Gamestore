using Business.Services.Concrete;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Moq;

namespace Test.BusinessTests
{
    public class CommentServiceTests
    {
        private readonly Mock<ICommentRepository> _mockCommentRepository;
        private readonly Mock<IGameRepository> _mockGameRepository;
        private readonly CommentService _service;

        public CommentServiceTests()
        {
            _mockCommentRepository = new Mock<ICommentRepository>();
            _mockGameRepository = new Mock<IGameRepository>();
            Mock<ILogger<CommentService>> mockLogger = new();
            _service = new CommentService(_mockCommentRepository.Object, _mockGameRepository.Object, mockLogger.Object);
        }

        [Fact]
        public async Task AddCommentAsync_ShouldThrow_WhenGameNotFound()
        {
            // Arrange
            string gameKey = "nonexistent";
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync((Game)null!);


            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.AddCommentAsync(gameKey, "John", "Test comment", null, null));
        }

        [Fact]
        public async Task AddCommentAsync_ShouldAddComment_WhenNoParentProvided()
        {
            // Arrange
            string gameKey = "game123";
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = gameKey,
                Name = "game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = default
            };
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync(game);
            _mockCommentRepository.Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddCommentAsync(gameKey, "John", "Test comment", null, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.Name);
            Assert.Equal("Test comment", result.Body);
        }

        [Fact]
        public async Task AddCommentAsync_ShouldFormatReply_WhenActionIsReply()
        {
            // Arrange
            string gameKey = "game123";
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = gameKey,
                Name = "game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = default
            };
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync(game);

            var parentComment = new Comment
            {
                Id = Guid.NewGuid(),
                Name = "ParentUser",
                Body = "Parent comment body",
                GameId = game.Id
            };
            _mockCommentRepository.Setup(x => x.GetByIdAsync(parentComment.Id))
                .ReturnsAsync(parentComment);
            _mockCommentRepository.Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddCommentAsync(gameKey, "John", "Reply text", parentComment.Id, "reply");

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"[{parentComment.Name}], Reply text", result.Body);
        }

        [Fact]
        public async Task AddCommentAsync_ShouldFormatQuote_WhenActionIsQuote()
        {
            // Arrange
            string gameKey = "game123";
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = gameKey,
                Name = "game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = default
            };
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync(game);

            var parentComment = new Comment
            {
                Id = Guid.NewGuid(),
                Name = "ParentUser",
                Body = "Parent comment body",
                GameId = game.Id
            };
            _mockCommentRepository.Setup(x => x.GetByIdAsync(parentComment.Id))
                .ReturnsAsync(parentComment);
            _mockCommentRepository.Setup(x => x.AddAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.AddCommentAsync(gameKey, "John", "Quote text", parentComment.Id, "quote");

            // Assert
            Assert.NotNull(result);
            Assert.Equal($"[{parentComment.Body}], Quote text", result.Body);
        }

        [Fact]
        public async Task GetCommentsByGameKeyAsync_ShouldThrow_WhenGameNotFound()
        {
            // Arrange
            string gameKey = "nonexistent";
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync((Game)null!);


            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.GetCommentsByGameKeyAsync(gameKey));
        }

        [Fact]
        public async Task GetCommentsByGameKeyAsync_ShouldReturnNestedComments()
        {
            // Arrange
            string gameKey = "game123";
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = gameKey,
                Name = "game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = default
            };
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync(game);

            var parentComment = new Comment
            {
                Id = Guid.NewGuid(),
                Name = "ParentUser",
                Body = "Parent comment",
                GameId = game.Id,
                ParentCommentId = null
            };
            var childComment = new Comment
            {
                Id = Guid.NewGuid(),
                Name = "ChildUser",
                Body = "Child comment",
                GameId = game.Id,
                ParentCommentId = parentComment.Id
            };

            var comments = new List<Comment> { parentComment, childComment };
            _mockCommentRepository.Setup(x => x.GetCommentsByGameIdAsync(game.Id))
                .ReturnsAsync(comments);

            // Act
            var result = await _service.GetCommentsByGameKeyAsync(gameKey);

            // Assert
            var list = result.ToList();
            Assert.Single(list);
            Assert.Single(list[0].ChildComments);
            Assert.Equal(childComment.Id, list[0].ChildComments.First().Id);
        }

        [Fact]
        public async Task DeleteCommentAsync_ShouldThrow_WhenGameNotFound()
        {
            // Arrange
            string gameKey = "nonexistent";
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync((Game)null!);


            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteCommentAsync(gameKey, Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteCommentAsync_ShouldThrow_WhenCommentNotFound()
        {
            // Arrange
            string gameKey = "game123";
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = gameKey,
                Name = "game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = default
            };
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync(game);

            _mockCommentRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Comment?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _service.DeleteCommentAsync(gameKey, Guid.NewGuid()));
        }

        [Fact]
        public async Task DeleteCommentAsync_ShouldUpdateCommentAndChildComments()
        {
            // Arrange
            string gameKey = "game123";
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Key = gameKey,
                Name = "game",
                Price = 0,
                UnitInStock = 0,
                Discount = 0,
                PublisherId = default
            };
            _mockGameRepository.Setup(x => x.GetByKeyAsync(gameKey))
                .ReturnsAsync(game);
            var commentToDelete = new Comment
            {
                Id = Guid.NewGuid(),
                Name = "John",
                Body = "Original comment",
                GameId = game.Id
            };

            var childComment = new Comment
            {
                Id = Guid.NewGuid(),
                Name = "Alice",
                Body = $"[{commentToDelete.Name}], Some reply text",
                GameId = game.Id,
                ParentCommentId = commentToDelete.Id
            };

            _mockCommentRepository.Setup(x => x.GetByIdAsync(commentToDelete.Id))
                .ReturnsAsync(commentToDelete);
            _mockCommentRepository.Setup(x => x.UpdateAsync(It.IsAny<Comment>()))
                .Returns(Task.CompletedTask);
            _mockCommentRepository.Setup(x => x.GetCommentsByParentIdAsync(commentToDelete.Id))
                .ReturnsAsync(new List<Comment> { childComment });

            // Act
            await _service.DeleteCommentAsync(gameKey, commentToDelete.Id);

            Assert.Equal("A comment/quote was deleted", commentToDelete.Body);
            Assert.StartsWith("[A comment/quote was deleted]", childComment.Body);
            _mockCommentRepository.Verify(x => x.UpdateAsync(commentToDelete), Times.Once);
            _mockCommentRepository.Verify(x => x.UpdateAsync(childComment), Times.Once);
        }

        [Fact]
        public async Task GetBanDurationsAsync_ShouldReturnExpectedDurations()
        {
            // Act
            var durations = await _service.GetBanDurationsAsync();

            // Assert
            var expected = new List<string> { "1 hour", "1 day", "1 week", "1 month", "permanent" };
            Assert.Equal(expected, durations.ToList());
        }

        [Fact]
        public async Task BanUserAsync_ShouldSetBanForUser()
        {
            // Act
            await _service.BanUserAsync("testUser", "1 day");

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.AddCommentAsync("game123", "testUser", "Test comment", null, null));
        }
    }
}
