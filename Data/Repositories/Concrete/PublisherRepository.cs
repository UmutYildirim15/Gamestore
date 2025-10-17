using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class PublisherRepository(ApplicationDbContext context) : IPublisherRepository
    {
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task<List<Publisher>> GetAllAsync()
        {
            return await context.Publishers
                .ToListAsync();
        }

        public async Task<Publisher?> GetByIdAsync(Guid id)
        {
            return await context.Publishers
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Publisher? publisher)
        {
            if (publisher != null) await context.Publishers.AddAsync(publisher);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync(Publisher? publisher)
        {
            if (publisher != null) context.Publishers.Update(publisher);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync(Publisher? publisher)
        {
            if (publisher != null) context.Publishers.Remove(publisher);
            await SaveChangesAsync();
        }

        public async Task<Publisher?> GetByNameAsync(string companyName)
        {
            companyName = companyName.Trim();
            return await context.Publishers
                .FirstOrDefaultAsync(p => p.CompanyName == companyName);
        }

        

        public async Task<List<Game>> GetGamesByPublisherIdAsync(Guid publisherId)
        {
            return await context.Games
                .Where(g => g.PublisherId == publisherId)
                .ToListAsync();
        }

        public async Task<List<Publisher>> SearchPublishersAsync(string keyword)
        {
            return await context.Publishers
                .Where(p => p.CompanyName.Contains(keyword))
                .ToListAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string companyName)
        {
            return !await context.Publishers.AnyAsync(p => p.CompanyName == companyName);
        }
    }
}
