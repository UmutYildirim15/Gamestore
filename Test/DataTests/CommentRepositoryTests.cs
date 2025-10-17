using Data.Entities;
using Data.Repositories.Concrete;
using Moq;
using Test.Helpers;
using Data;

namespace Test.DataTests
{
    public class CommentRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly CommentRepository _repository;

        public CommentRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _repository = new CommentRepository(_contextMock.Object);
        }

        [Fact]
        public async Task AddAsync_ShouldAddComment()
        {
            // Arrange
            var comment = DataTestsHelper.CreateComment(
                id: Guid.NewGuid(), 
                name: "John", 
                body: "Test comment", 
                parentId: null, 
                gameId: Guid.NewGuid());
            
            DataTestsHelper.CreateMockDbSetForComments(new List<Comment>(), _contextMock);

            // Act
            await _repository.AddAsync(comment);

            // Assert
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnComment_WhenExists()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            var comment = DataTestsHelper.CreateComment(
                id: commentId, 
                name: "John", 
                body: "Test comment", 
                parentId: null, 
                gameId: Guid.NewGuid());
            
            DataTestsHelper.CreateMockDbSetForComments(new List<Comment> { comment }, _contextMock);

            // Act
            var result = await _repository.GetByIdAsync(commentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(commentId, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var commentId = Guid.NewGuid();
            DataTestsHelper.CreateMockDbSetForComments(new List<Comment>(), _contextMock);

            // Act
            var result = await _repository.GetByIdAsync(commentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateComment()
        {
            // Arrange
            var comment = DataTestsHelper.CreateComment(
                id: Guid.NewGuid(), 
                name: "John", 
                body: "Initial comment", 
                parentId: null, 
                gameId: Guid.NewGuid());
            
            DataTestsHelper.CreateMockDbSetForComments(new List<Comment> { comment }, _contextMock);

            // Act
            comment.Body = "Updated comment";
            await _repository.UpdateAsync(comment);

            // Assert
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveComment()
        {
            // Arrange
            var comment = DataTestsHelper.CreateComment(
                id: Guid.NewGuid(), 
                name: "John", 
                body: "Comment to delete", 
                parentId: null, 
                gameId: Guid.NewGuid());
            
            DataTestsHelper.CreateMockDbSetForComments(new List<Comment> { comment }, _contextMock);

            // Act
            await _repository.DeleteAsync(comment);

            // Assert
            _contextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GetCommentsByGameIdAsync_ShouldReturnComments()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var comment1 = DataTestsHelper.CreateComment(
                id: Guid.NewGuid(), 
                name: "User1", 
                body: "Comment 1", 
                parentId: null, 
                gameId: gameId);
            var comment2 = DataTestsHelper.CreateComment(
                id: Guid.NewGuid(), 
                name: "User2", 
                body: "Comment 2", 
                parentId: null, 
                gameId: gameId);
            
            DataTestsHelper.CreateMockDbSetForComments(new List<Comment> { comment1, comment2 }, _contextMock);

            // Act
            var results = await _repository.GetCommentsByGameIdAsync(gameId);

            // Assert
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public async Task GetCommentsByParentIdAsync_ShouldReturnComments()
        {
            // Arrange
            var parentId = Guid.NewGuid();
            var comment1 = DataTestsHelper.CreateComment(
                id: Guid.NewGuid(), 
                name: "User1", 
                body: "Child comment 1", 
                parentId: parentId, 
                gameId: Guid.NewGuid());
            var comment2 = DataTestsHelper.CreateComment(
                id: Guid.NewGuid(), 
                name: "User2", 
                body: "Child comment 2", 
                parentId: parentId, 
                gameId: Guid.NewGuid());
            
            DataTestsHelper.CreateMockDbSetForComments(new List<Comment> { comment1, comment2 }, _contextMock);

            // Act
            var results = await _repository.GetCommentsByParentIdAsync(parentId);

            // Assert
            Assert.Equal(2, results.Count());
        }
    }
}
