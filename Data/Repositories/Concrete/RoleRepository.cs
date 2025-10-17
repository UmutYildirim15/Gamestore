using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete;

public class RoleRepository(ApplicationDbContext context): IRoleRepository
{
    public async Task<IEnumerable<Role>> GetAllAsync()
    {
        return await context.Roles.ToListAsync();
    }

    public async Task<Role> GetByIdAsync(Guid id)
    {
        return await context.Roles.FirstOrDefaultAsync(r => r.Id == id) ?? throw new InvalidOperationException();
    }

    public async Task AddAsync(Role role)
    {
        await context.Roles.AddAsync(role);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Role role)
    {
        context.Roles.Update(role);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var role = await context.Roles.FindAsync(id);
        if (role != null)
        {
            context.Roles.Remove(role);
            await context.SaveChangesAsync();
        }
    }
    
    public async Task<IEnumerable<string>> GetPermissionsByRoleIdAsync(Guid roleId)
    {
        return await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission.Name)
            .ToListAsync();
    }

}