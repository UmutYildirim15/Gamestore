using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class UserRepository(ApplicationDbContext context) : IUserRepository
    {
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ToListAsync();
        }
    
        public async Task<User> GetByIdAsync(Guid id)
        {
            return await context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id) ?? throw new InvalidOperationException();
        }
        
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await context.Users
                .Where(u => u.Name == username)
                .FirstOrDefaultAsync() ?? throw new InvalidOperationException();
        }
    
        public async Task AddAsync(User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }
    
        public async Task UpdateAsync(User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }
    
        public async Task DeleteAsync(Guid id)
        {
            var userRoles = await context.UserRoles.Where(ur => ur.UserId == id).ToListAsync();
            if (userRoles.Any())
            {
                context.UserRoles.RemoveRange(userRoles);
            }
            var user = await context.Users.FindAsync(id);
            if (user != null)
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }
        }
    
        public async Task<IEnumerable<Role?>> GetUserRolesAsync(Guid id)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
            return user?.UserRoles?.Select(ur => ur.Role) ?? new List<Role>();
        }
    }
}

