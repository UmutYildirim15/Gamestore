using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre?> GetByIdAsync(Guid id);
        Task<IEnumerable<Genre>> GetByIdsAsync(IEnumerable<Guid> genreIds);
        Task AddAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Genre>> GetGenresByParentIdAsync(Guid parentId);
        Task SaveChangesAsync();
        Task<Genre?> GetByNameAsync(string name);


    }
}
