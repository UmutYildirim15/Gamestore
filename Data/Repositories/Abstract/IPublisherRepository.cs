using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IPublisherRepository
    {
        Task SaveChangesAsync();
        Task<List<Publisher>> GetAllAsync();
        Task<Publisher?> GetByIdAsync(Guid id);
        Task AddAsync(Publisher? publisher);
        Task UpdateAsync(Publisher? publisher);
        Task DeleteAsync(Publisher? publisher);
        Task<Publisher?> GetByNameAsync(string companyName);
        Task<List<Game>> GetGamesByPublisherIdAsync(Guid publisherId);
        Task<List<Publisher>> SearchPublishersAsync(string keyword);
        Task<bool> IsNameUniqueAsync(string companyName);
    }
}