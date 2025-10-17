using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role> GetByIdAsync(Guid id);
        Task AddAsync(Role role);
        Task UpdateAsync(Role role);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<string>> GetPermissionsByRoleIdAsync(Guid roleId);
    }
}