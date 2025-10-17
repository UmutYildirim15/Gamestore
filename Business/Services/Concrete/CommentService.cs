using Business.Services.Abstract;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class CommentService(
        ICommentRepository commentRepository,
        IGameRepository gameRepository,
        ILogger<ICommentService> logger)
        : ICommentService
    {
        private static readonly Dictionary<string, DateTime?> BannedUsers = new();

        private static readonly List<string> BanDurations = ["1 hour", "1 day", "1 week", "1 month", "permanent"];

        public async Task<CommentResponse> AddCommentAsync(string gameKey, string name, string body, Guid? parentId, string? action)
        {
            if (BannedUsers.TryGetValue(name, out var banExpiry))
            {
                if (banExpiry == null || banExpiry > DateTime.UtcNow)
                    throw new InvalidOperationException("User is banned from commenting.");
                BannedUsers.Remove(name);
            }
            var game = await gameRepository.GetByKeyAsync(gameKey);
            if (game == null)
                throw new KeyNotFoundException($"Game with key {gameKey} not found.");

            var newComment = new Comment
            {
                Id = Guid.NewGuid(),
                Name = name, 
                Body = body,
                GameId = game.Id,
                ParentCommentId = parentId
            };
            if (parentId != null && !string.IsNullOrWhiteSpace(action))
            {
                var parentComment = await commentRepository.GetByIdAsync(parentId.Value);
                if (parentComment == null)
                    throw new KeyNotFoundException("Parent comment not found.");

                if (action.ToLower() == "reply")
                    newComment.Body = $"[{parentComment.Name}], {body}";
                else if (action.ToLower() == "quote")
                    newComment.Body = $"[{parentComment.Body}], {body}";
            }

            await commentRepository.AddAsync(newComment);
            logger.LogInformation("Comment added for game {GameKey} by {UserName}.", gameKey, name);
    
            return new CommentResponse
            {
                Id = newComment.Id,
                Name = newComment.Name,
                Body = newComment.Body,
                ChildComments = new List<CommentResponse>()
            };
        }


        public async Task<IEnumerable<CommentResponse>> GetCommentsByGameKeyAsync(string gameKey)
        {
            var game = await gameRepository.GetByKeyAsync(gameKey);
            if (game == null)
                throw new KeyNotFoundException($"Game with key {gameKey} not found.");
            var comments = await commentRepository.GetCommentsByGameIdAsync(game.Id);
            var enumerable = comments.ToList();
            var commentResponses = enumerable.ToDictionary(c => c.Id, c => new CommentResponse
            {
                Id = c.Id,
                Name = c.Name,
                Body = c.Body,
                ChildComments = new List<CommentResponse>()
            });
            var result = new List<CommentResponse>();
            foreach (var comment in commentResponses.Values)
            {
                var original = enumerable.FirstOrDefault(c => c.Id == comment.Id);
                if (original?.ParentCommentId != null && commentResponses.ContainsKey(original.ParentCommentId.Value))
                {
                    commentResponses[original.ParentCommentId.Value].ChildComments.Add(comment);
                }
                else
                {
                    result.Add(comment);
                }
            }
            return result;
        }

        public async Task DeleteCommentAsync(string gameKey, Guid commentId)
        {
            var game = await gameRepository.GetByKeyAsync(gameKey);
            if (game == null)
                throw new KeyNotFoundException($"Game with key {gameKey} not found.");

            var comment = await commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                throw new KeyNotFoundException("Comment not found.");

            comment.Body = "A comment/quote was deleted";
            await commentRepository.UpdateAsync(comment);
            logger.LogInformation("Comment {CommentId} deleted.", commentId);

            var childComments = await commentRepository.GetCommentsByParentIdAsync(commentId);
            foreach (var child in childComments)
            {
                int start = child.Body.IndexOf('[');
                int end = child.Body.IndexOf(']');
                if (start >= 0 && end > start)
                {
                    var remainder = child.Body.Substring(end + 1);
                    child.Body = "[A comment/quote was deleted]" + remainder;
                    await commentRepository.UpdateAsync(child);
                }
            }
        }
        public Task<IEnumerable<string>> GetBanDurationsAsync()
        {
            return Task.FromResult<IEnumerable<string>>(BanDurations);
        }

        public Task BanUserAsync(string user, string duration)
        {
            DateTime? banExpiry = duration.ToLower() switch
            {
                "1 hour" => DateTime.UtcNow.AddHours(1),
                "1 day" => DateTime.UtcNow.AddDays(1),
                "1 week" => DateTime.UtcNow.AddDays(7),
                "1 month" => DateTime.UtcNow.AddMonths(1),
                "permanent" => null, // null represents a permanent ban.
                _ => throw new ArgumentException("Invalid ban duration"),
            };

            BannedUsers[user] = banExpiry;
            logger.LogInformation("User {User} banned for {Duration}.", user, duration);
            return Task.CompletedTask;
        }
    }
}
