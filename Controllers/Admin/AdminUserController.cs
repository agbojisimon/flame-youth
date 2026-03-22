using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.User;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public AdminUserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET /api/admin/users
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryObject query)
        {
            var users = await _userService.GetUsersAsync(query);
            return Ok(users);
        }

        // GET /api/admin/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        // POST /api/admin/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RegisterDto dto)
        {
            var user = await _userService.CreateUserAsync(dto);
            return Ok(user);
        }

        // PUT /api/admin/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userService.UpdateUserAsync(id, dto);
            if (user == null) return NotFound("User not found");
            return Ok(user);
        }

        // DELETE /api/admin/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted) return NotFound("User not found");
            return Ok("User deleted successfully");
        }

        // POST /api/admin/users/assign-role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto dto)
        {
            var result = await _userService.AssignRoleAsync(dto.UserId, dto.Role);

            if (!result)
                return NotFound("User not found");

            return Ok("Role assigned successfully");
        }
    }
}