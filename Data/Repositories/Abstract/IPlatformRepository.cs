using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IPlatformRepository
    {
        Task<IEnumerable<Platform>> GetAllAsync();
        Task<Platform?> GetByIdAsync(Guid id);
        Task<IEnumerable<Platform>> GetByIdsAsync(IEnumerable<Guid> platformIds);
        Task AddAsync(Platform platform);
        Task UpdateAsync(Platform platform);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
