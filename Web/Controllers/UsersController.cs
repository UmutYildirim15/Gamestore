using Business.Services.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Web.Models.CreateModels;
using Web.Models.RequestModels;
using Web.Utilities;

namespace Web.Controllers
{
    [ApiController]
    [Route("users")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userService.GetAllUsersAsync();
            return Ok(users);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await userService.GetUserByIdAsync(id);
            return Ok(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserCreate userCreate)
        {
            await userService.AddUserAsync(userCreate.Name, userCreate.Password, userCreate.RoleIds);
            return CreatedAtAction(nameof(GetUserById), new { id = userCreate.Id }, userCreate);
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserRequest userRequest)
        {
            userRequest.Id = id;
            await userService.UpdateUserAsync(userRequest.Id, userRequest.Name, userRequest.Password, userRequest.RoleIds);
            return NoContent();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await userService.DeleteUserAsync(id);
            return NoContent();
        }
        
        [HttpGet("{id:guid}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid id)
        {
            var roles = await userService.GetUserRolesAsync(id);
            return Ok(roles);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var token = await userService.LoginAsync(loginRequest.Login, loginRequest.Password, loginRequest.InternalAuth);
            return Ok(new { token });
        }
        
        [HttpPost("access")]
        public async Task<IActionResult> CheckAccess([FromBody] CheckAccessRequest checkAccess)
        {
            if (!Guid.TryParse(checkAccess.TargetId, out Guid targetId))
                return BadRequest($"Invalid TargetId format: {checkAccess.TargetId}");
            var access = checkAccess.TargetPage != null && await userService.CheckAccessAsync(targetId, checkAccess.TargetPage);
            return Ok(new { Access = access });
        }
    }
}

