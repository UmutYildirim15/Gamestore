using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class GameRepository(ApplicationDbContext context) : IGameRepository
    {
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<List<Game>> GetAllAsync()
        {
            return await context.Games
                .Include(g => g.Genres!).ThenInclude(gg => gg.Genre)
                .Include(g => g.Platforms!).ThenInclude(pg => pg.Platform)
                .ToListAsync();
        }

        public async Task<Game> GetByIdAsync(Guid id)
        {
            var game = await context.Games
                           .Include(g => g.Genres)
                           .Include(g => g.Platforms)
                           .FirstOrDefaultAsync(g => g.Id == id) 
                       ?? throw new InvalidOperationException();
            game.ViewCount++; // Nonfunctional requirement 1
            await context.SaveChangesAsync();
            return game;
        }

        public async Task AddAsync(Game game)
        {
            await context.Games.AddAsync(game);
            await SaveChangesAsync(); 
        }

        public async Task UpdateAsync(Game game)
        {
            context.Games.Update(game);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Game game)
        {
            context.Games.Remove(game);
            await SaveChangesAsync();
        }

        public async Task<Game> GetByKeyAsync(string key)
        {
            return await context.Games
                .Include(g => g.Genres)
                .Include(g => g.Platforms)
                .FirstOrDefaultAsync(g => g.Key == key) ?? throw new InvalidOperationException();
        }
        
        public async Task<List<Game?>> GetGamesByGenreAsync(Guid genreId)
        {
            return await context.GameGenres
                .Where(gg => gg.GenreId == genreId)
                .Select(gg => gg.Game)
                .ToListAsync();
        }
        
        public async Task<List<Game?>> GetGamesByPlatformAsync(Guid platformId)
        {
            return await context.GamePlatforms
                .Where(gp => gp.PlatformId == platformId)
                .Select(gp => gp.Game)
                .ToListAsync();
        }
        
        public async Task<List<string>> GetAllKeysStartingWithAsync(string baseKey)
        {
            return await context.Games
                .Where(g => g.Key.StartsWith(baseKey))
                .Select(g => g.Key)
                .ToListAsync();
        }
        
        
    }
}
