using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class GameGenreRepository(ApplicationDbContext context) : IGameGenreRepository
    {
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<GameGenre>> GetGameGenresByGameAsync(Guid gameId)
        {
            var gameGenres = await context.GameGenres
                .Where(g => g.GameId == gameId)
                .Include(g => g.Genre)
                .ToListAsync();

            return gameGenres; 
        }
        
        public async Task RemoveGameGenresAsync(IEnumerable<GameGenre>? gameGenres)
        {
            if (gameGenres != null) context.GameGenres.RemoveRange(gameGenres);
            await SaveChangesAsync();
        }
    }
}