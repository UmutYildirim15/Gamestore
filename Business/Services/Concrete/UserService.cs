using System.Net.Http.Json;
using Business.Services.Abstract;
using Business.Services.Resources;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class UserService(
        IUserRepository userRepo,
        IRoleRepository roleRepo,
        IRoleService roleService,
        IConfiguration configuration,
        HttpClient httpClient,
        ILogger<IGameService> logger)
        : IUserService
    {
        private readonly string _authServiceUrl = configuration["Services:AuthService"]
                                                 ?? throw new ArgumentNullException("Url not found");

        public async Task<string?> LoginAsync(string login, string password, bool internalAuth)
        {
            if (!internalAuth)
                throw new UnauthorizedAccessException("Invalid credentials");

            var users = await userRepo.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Name.Equals(login, StringComparison.OrdinalIgnoreCase));

            if (user != null && password == user.Password)
            {
                var authRequest = new AuthRequest
                {
                    Username = login,
                    Password = password
                };

                var response = await httpClient.PostAsJsonAsync($"{_authServiceUrl}/token", authRequest);

                if (!response.IsSuccessStatusCode)
                    throw new UnauthorizedAccessException("AuthService failed");

                var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
                logger.LogWarning("A user is logged in!");
                return authResponse?.Token ?? throw new UnauthorizedAccessException("Invalid response from AuthService");
            }

            throw new UnauthorizedAccessException("Invalid credentials");
        }



        public async Task<bool> CheckAccessAsync(Guid targetId, string targetPage)
        {
            var user = await userRepo.GetByIdAsync(targetId);
            if (!user.UserRoles.Any())
                return false;

            var roleIds = user.UserRoles.Select(ur => ur.RoleId);
            var allPermissions = new List<string>();

            foreach (var roleId in roleIds)
            {
                var rolePermissions = await roleService.GetRolePermissionsAsync(roleId);
                allPermissions.AddRange(rolePermissions);
            }

            return allPermissions.Contains(targetPage, StringComparer.OrdinalIgnoreCase);
        }
        
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await userRepo.GetAllAsync();
            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Password = u.Password,
                RoleIds = u.UserRoles.Select(ur => ur.RoleId) ?? throw new InvalidOperationException()
            });
        }

        public async Task<UserDto> GetUserByIdAsync(Guid id)
        {
            var user = await userRepo.GetByIdAsync(id);
            if (user == null)
                throw new Exception(ErrorMessages.UserNotFound);
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Password = user.Password,
                RoleIds = user.UserRoles.Select(ur => ur.RoleId) ?? throw new InvalidOperationException()
            };
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await userRepo.DeleteAsync(id);
        }

        public async Task AddUserAsync(string name, string password, IEnumerable<Guid> roleIds)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = name,
                Password = password 
            };

            user.UserRoles = roleIds.Select(roleId => new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                User = user
            }).ToList();

            await userRepo.AddAsync(user);
        }

        public async Task UpdateUserAsync(Guid id, string name, string password, IEnumerable<Guid> roleIds)
        {
            var user = await userRepo.GetByIdAsync(id);
            if (user == null)
                throw new Exception(ErrorMessages.UserNotFound);

            user.Name = name;
            user.Password = password;

            user.UserRoles = roleIds.Select(roleId => new UserRole
            {
                UserId = user.Id,
                RoleId = roleId,
                User = user
            }).ToList();

            await userRepo.UpdateAsync(user);
        }

        public async Task<IEnumerable<RoleDto?>> GetUserRolesAsync(Guid id)
        {
            var roles = await userRepo.GetUserRolesAsync(id);
            return roles.Select(r =>
            {
                if (r != null) return new RoleDto { Id = r.Id, Name = r.Name };
                return null;
            });
        }
    }
}
