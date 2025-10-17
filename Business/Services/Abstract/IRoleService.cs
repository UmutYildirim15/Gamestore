using Shared.Dto;

namespace Business.Services.Abstract
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllRolesAsync();
        Task<RoleDto> GetRoleByIdAsync(Guid id);
        Task DeleteRoleAsync(Guid id);
        Task AddRoleAsync(Guid id, string name, IEnumerable<string> permissions);
        Task UpdateRoleAsync(Guid id, string name, IEnumerable<string> permissions);
        Task<IEnumerable<string>> GetPermissionsAsync();
        Task<IEnumerable<string>> GetRolePermissionsAsync(Guid id);
    }
}