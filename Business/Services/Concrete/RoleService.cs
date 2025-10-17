using Business.Services.Abstract;
using Business.Services.Resources;
using Data.Entities;
using Data.Repositories.Abstract;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class RoleService(IRoleRepository roleRepo) : IRoleService
    {
        private readonly List<string> _allPermissions = ["AddGame", "DeleteGame", "ViewGame", "UpdateGame"];
        private readonly Dictionary<Guid, List<string>> _rolePermissions = new();

        public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
        {
            var roles = await roleRepo.GetAllAsync();
            return roles.Select(r => new RoleDto { Id = r.Id, Name = r.Name });
        }

        public async Task<RoleDto> GetRoleByIdAsync(Guid id)
        {
            var role = await roleRepo.GetByIdAsync(id);
            if (role == null)
                throw new Exception(ErrorMessages.RoleNotFound);
            return new RoleDto { Id = role.Id, Name = role.Name };
        }

        public async Task DeleteRoleAsync(Guid id)
        {
            await roleRepo.DeleteAsync(id);
        }

        public async Task AddRoleAsync(Guid id, string name, IEnumerable<string> permissions)
        {
            var role = new Role
            {
                Id = id,
                Name = name
            };
            await roleRepo.AddAsync(role);
            _rolePermissions[role.Id] = permissions.ToList();
        }


        public async Task UpdateRoleAsync(Guid id, string name, IEnumerable<string> permissions)
        {
            var role = await roleRepo.GetByIdAsync(id);
            if (role == null)
                throw new Exception(ErrorMessages.RoleNotFound);
            role.Name = name;
            await roleRepo.UpdateAsync(role);
            _rolePermissions[role.Id] = permissions.ToList();
        }

        public Task<IEnumerable<string>> GetPermissionsAsync()
        {
            return Task.FromResult<IEnumerable<string>>(_allPermissions);
        }

        public Task<IEnumerable<string>> GetRolePermissionsAsync(Guid id)
        {
            return Task.FromResult<IEnumerable<string>>(_rolePermissions.TryGetValue(id, out var perms) ? perms : []);
        }
    }
}
