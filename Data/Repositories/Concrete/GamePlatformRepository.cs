using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class GamePlatformRepository(ApplicationDbContext context) : IGamePlatformRepository
    {
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
        
        public async Task<IEnumerable<GamePlatform>> GetGamePlatformsByGameAsync(Guid gameId)
        {
            return await context.GamePlatforms
                .Where(gp => gp.GameId == gameId)
                .ToListAsync();
        }
        public async Task RemoveGamePlatformsAsync(IEnumerable<GamePlatform>? gamePlatforms)
        {
            if (gamePlatforms != null) context.GamePlatforms.RemoveRange(gamePlatforms);
            await SaveChangesAsync();
        }
    }
}