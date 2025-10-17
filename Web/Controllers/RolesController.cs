using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Web.Models.CreateModels;
using Web.Models.RequestModels;
using Web.Utilities;

namespace Web.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await roleService.GetAllRolesAsync();
            return Ok(roles);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetRole([FromRoute] Guid id)
        {
            var role = await roleService.GetRoleByIdAsync(id);
            return Ok(role);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] RoleCreate roleCreate)
        {
            await roleService.AddRoleAsync(roleCreate.Id, roleCreate.Name, roleCreate.Permissions);
            return CreatedAtAction(nameof(GetRole), new { id = roleCreate.Id }, roleCreate);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateRole(
            [FromRoute] Guid id,
            [FromBody] RoleRequest roleRequest)
        {
            roleRequest.Id = id;
            await roleService.UpdateRoleAsync(roleRequest.Id, roleRequest.Name, roleRequest.Permissions);
            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteRole([FromRoute] Guid id)
        {
            await roleService.DeleteRoleAsync(id);
            return NoContent();
        }
        
        [HttpGet("permissions")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await roleService.GetPermissionsAsync();
            return Ok(permissions);
        }
        
        [HttpGet("{id:guid}/permissions")]
        public async Task<IActionResult> GetRolePermissions([FromRoute] Guid id)
        {
            var permissions = await roleService.GetRolePermissionsAsync(id);
            return Ok(permissions);
        }
    }
}
