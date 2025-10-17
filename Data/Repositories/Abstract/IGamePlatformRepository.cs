using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IGamePlatformRepository
    {
        Task<IEnumerable<GamePlatform>> GetGamePlatformsByGameAsync(Guid gameId);
        Task RemoveGamePlatformsAsync(IEnumerable<GamePlatform>? gamePlatforms);
        Task SaveChangesAsync();
    }
}    