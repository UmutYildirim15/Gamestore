using Shared.Dto;

namespace Business.Services.Abstract
{
    public interface IUserService
    {
        Task<string?> LoginAsync(string login, string password, bool internalAuth);
        Task<bool> CheckAccessAsync(Guid targetId, string targetPage);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserByIdAsync(Guid id);
        Task DeleteUserAsync(Guid id);
        Task AddUserAsync(string name, string password, IEnumerable<Guid> roleIds);
        Task UpdateUserAsync(Guid id, string name, string password, IEnumerable<Guid> roleIds);
        Task<IEnumerable<RoleDto?>> GetUserRolesAsync(Guid id);
    }
}