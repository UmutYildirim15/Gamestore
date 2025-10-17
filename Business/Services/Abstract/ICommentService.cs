using Shared.Dto;

namespace Business.Services.Abstract;

public interface ICommentService
{
    Task<CommentResponse> AddCommentAsync(string gameKey, string name, string body, Guid? parentId, string? action);
    Task<IEnumerable<CommentResponse>> GetCommentsByGameKeyAsync(string gameKey);
    Task DeleteCommentAsync(string gameKey, Guid commentId);
    Task<IEnumerable<string>> GetBanDurationsAsync();
    Task BanUserAsync(string user, string duration);
}