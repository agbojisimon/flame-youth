using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.User;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("g-flame-youth/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryObject query)
        {
            var users = await _userService.GetUsersAsync(query);
            return Ok(users);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest("User ID is required.");

            var user = await _userService.GetUserByIdAsync(Id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.CreateUserAsync(registerDto);
            return Ok(user);
        }

        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string Id, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = await _userService.UpdateUserAsync(Id, updateUserDto);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string Id)
        {
            var deleted = await _userService.DeleteUserAsync(Id);
            if (!deleted)
                return NotFound();

            return Ok("User deleted successfully.");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var result = await _userService.AssignRoleAsync(assignRoleDto.userId, assignRoleDto.Role);

            if (!result)
                return BadRequest("Failed to assign role.");

            return Ok("Role assigned successfully.");
        }
    }
}