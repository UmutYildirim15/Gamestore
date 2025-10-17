using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class PlatformRepository(ApplicationDbContext context) : IPlatformRepository
    {
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Platform>> GetAllAsync()
        {
            return await context.Platforms.ToListAsync();
        }
        public async Task<Platform?> GetByIdAsync(Guid id)
        {
            return await context.Platforms.FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<IEnumerable<Platform>> GetByIdsAsync(IEnumerable<Guid> platformIds)
        {
            return await context.Platforms
                .Where(p => platformIds.Contains(p.Id))
                .ToListAsync();
        }
        public async Task AddAsync(Platform platform)
        {
            await context.Platforms.AddAsync(platform);
            await SaveChangesAsync();
        }
        public async Task UpdateAsync(Platform platform)
        {
            context.Platforms.Update(platform);
            await SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var platform = await GetByIdAsync(id);
            if (platform != null)
            {
                context.Platforms.Remove(platform);
            }
            await SaveChangesAsync();
        }
        
        
    }
}
