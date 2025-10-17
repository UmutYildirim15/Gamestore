using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class CommentRepository(ApplicationDbContext context) : ICommentRepository
    {
        public async Task AddAsync(Comment comment)
        {
            await context.Comments.AddAsync(comment);
            await context.SaveChangesAsync();
        }

        public async Task<Comment?> GetByIdAsync(Guid id)
        {
            return await context.Comments.FindAsync(id);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByGameIdAsync(Guid gameId)
        {
            return await context.Comments
                .Where(c => c.GameId == gameId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Comment>> GetCommentsByParentIdAsync(Guid parentCommentId)
        {
            return await context.Comments
                .Where(c => c.ParentCommentId == parentCommentId)
                .ToListAsync();
        }

        public async Task UpdateAsync(Comment comment)
        {
            context.Comments.Update(comment);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Comment comment)
        {
            context.Comments.Remove(comment);
            await context.SaveChangesAsync();
        }
    }
}