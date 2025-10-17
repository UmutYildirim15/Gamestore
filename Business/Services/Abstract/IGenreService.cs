using Shared.Dto;

namespace Business.Services.Abstract
{
    public interface IGenreService
    {
        Task<IEnumerable<GenreResponse>> GetAllGenresAsync();
        Task<Guid> CreateGenreAsync(string name, string? parentGenreName);
        Task<GenreResponse?> GetGenreByIdAsync(Guid id);
        Task<bool> UpdateGenreAsync(Guid id, string name);
        Task<bool> DeleteGenreAsync(Guid id);
        Task<IEnumerable<GenreResponse?>> GetGenresByGameKeyAsync(string key);
        Task<IEnumerable<GenreResponse>> GetGenresByParentIdAsync(Guid parentId);
    }
}