using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class GenreRepository(ApplicationDbContext context) : IGenreRepository
    {
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await context.Genres.ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(Guid id)
        {
            return await context.Genres
                .FirstOrDefaultAsync(g => g.Id == id);
        }
        
        public async Task<IEnumerable<Genre>> GetByIdsAsync(IEnumerable<Guid> genreIds)
        {
            return await context.Genres
                .Where(g => genreIds.Contains(g.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Genre genre)
        {
            await context.Genres.AddAsync(genre);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            context.Genres.Update(genre);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var genre = await GetByIdAsync(id);
            if (genre != null)
            {
                context.Genres.Remove(genre);
            }
            await SaveChangesAsync();
        }
        
        public async Task<IEnumerable<Genre>> GetGenresByParentIdAsync(Guid parentId)
        {
            return await context.Genres
                .Where(g => g.ParentGenreId == parentId)
                .ToListAsync();
        }
        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await context.Genres.FirstOrDefaultAsync(g => g.Name == name);
        }

    }
}
