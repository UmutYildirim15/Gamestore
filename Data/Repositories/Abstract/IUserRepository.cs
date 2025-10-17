using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> GetByIdAsync(Guid id);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Role?>> GetUserRolesAsync(Guid id);
        Task<User> GetByUsernameAsync(string username);
    }
}