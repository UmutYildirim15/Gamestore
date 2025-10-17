using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IGameRepository
    {
        Task<List<Game>> GetAllAsync();
        Task<Game> GetByIdAsync(Guid id);
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(Game game);
        Task<Game> GetByKeyAsync(string key);
        Task<List<Game?>> GetGamesByGenreAsync(Guid genreId);
        Task<List<Game?>> GetGamesByPlatformAsync(Guid platformId);
        Task<List<string>> GetAllKeysStartingWithAsync(string baseKey);
        Task SaveChangesAsync();
    }
}
