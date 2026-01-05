using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.User;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryObject query)
        {
            var users = await _userService.GetUsersAsync(query);

            return Ok(new ApiResponse<List<UserDto>>
            {
                isSuccess = true,
                Message = "User retrieved successfully",
                Data = users
            });
        }

        [HttpGet("{Id}/get-user")]
        public async Task<IActionResult> GetById([FromRoute] string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest(new ApiResponse<string?>
                {
                    isSuccess = false,
                    Message = "User ID cannot be null or empty.",
                    Data = null
                });

            var user = await _userService.GetUserByIdAsync(Id);

            if (user == null)
                return NotFound();

            return Ok(new ApiResponse<UserDto?>
            {
                isSuccess = true,
                Message = "User retrieved successfully",
                Data = user
            });
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.CreateUserAsync(registerDto);
            return Ok(new ApiResponse<UserDto>
            {
                isSuccess = true,
                Message = "User created successfully",
                Data = user
            });
        }

        [HttpPut("{Id}/update-user")]
        public async Task<IActionResult> UpdateUser([FromRoute] string Id, [FromBody] UpdateUserDto updateUserDto)
        {
            var user = await _userService.UpdateUserAsync(Id, updateUserDto);

            if (user == null)
                return NotFound();

            return Ok(new ApiResponse<UserDto>
            {
                isSuccess = true,
                Message = "User updated successfully",
                Data = user
            });
        }

        [HttpDelete("{Id}/delete-user")]
        public async Task<IActionResult> DeleteUser([FromRoute] string Id)
        {
            var deleted = await _userService.DeleteUserAsync(Id);
            if (!deleted)
                return NotFound();

            return Ok("User deleted successfully.");
        }

        [HttpPost("change-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var result = await _userService.AssignRoleAsync(assignRoleDto.userId, assignRoleDto.Role);

            if (!result)
                return BadRequest("Failed to assign role.");

            return Ok("Role assigned successfully.");
        }
    }
}