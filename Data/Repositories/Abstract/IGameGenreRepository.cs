using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IGameGenreRepository
    {
        Task<IEnumerable<GameGenre>> GetGameGenresByGameAsync(Guid gameId);
        Task RemoveGameGenresAsync(IEnumerable<GameGenre>? gameGenres);
        Task SaveChangesAsync();
    }
}
