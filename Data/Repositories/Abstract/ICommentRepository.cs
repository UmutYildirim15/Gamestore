using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Comment>> GetCommentsByGameIdAsync(Guid gameId);
        Task<IEnumerable<Comment>> GetCommentsByParentIdAsync(Guid parentCommentId);
        Task UpdateAsync(Comment comment);
        Task DeleteAsync(Comment comment);
    }
}