using g_flame_youth.Data;
using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.User;
using g_flame_youth.Helpers;
using g_flame_youth.Mappers;
using g_flame_youth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("g-flame-youth/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public UserController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] UserQueryObject query)
        {
            var usersQuery = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(query.Email));
            }

            if (!string.IsNullOrWhiteSpace(query.FullName))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.FirstName + " " + u.LastName).Contains(query.FullName));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Email", StringComparison.OrdinalIgnoreCase))
                {
                    usersQuery = query.IsDescending ? usersQuery.OrderByDescending(u => u.Email) : usersQuery.OrderBy(u => u.Email);
                }
            }
            else
            {
                usersQuery = usersQuery.OrderByDescending(u => u.CreatedOn);
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            var users = await usersQuery.Skip(skipNumber).Take(query.PageSize).ToListAsync();

            var userDtos = users.Select(u => u.ToUserDto()).ToList();

            return Ok(userDtos);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetById([FromRoute] string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest("User ID is required.");

            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
                return NotFound($"User with ID {Id} not found.");

            var userDto = user.ToUserDto();

            return Ok(userDto);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var appUser = new AppUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedOn = DateTime.UtcNow
            };

            var createdUser = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (!createdUser.Succeeded)
                return BadRequest(createdUser.Errors);

            var roleResult = await _userManager.AddToRoleAsync(appUser, "Member");

            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);

            var userDto = appUser.ToUserDto();

            return Ok(userDto);
        }
        [HttpPut("{Id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] string Id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest("User ID is required.");

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound($"User with ID {Id} not found.");

            user = updateUserDto.ToAppUser(user);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return BadRequest(updateResult.Errors);

            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateUserDto.Password);

                if (!passwordResult.Succeeded)
                    return BadRequest(passwordResult.Errors);
            }

            return Ok(user.ToUserDto());
        }

        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteUser([FromRoute] string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest("User ID is required.");

            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return NotFound($"User with ID {Id} not found.");

            var deleteResult = await _userManager.DeleteAsync(user);
            if (!deleteResult.Succeeded)
                return BadRequest(deleteResult.Errors);

            return Ok("User deleted successfully.");
        }

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto assignRoleDto)
        {
            var allowedRoles = new[] { "Member", "Admin" };
            if (!allowedRoles.Contains(assignRoleDto.Role))
                return BadRequest("Invalid Role");

            var user = await _userManager.FindByIdAsync(assignRoleDto.userId);
            if (user == null)
                return NotFound($"User with ID {assignRoleDto.userId} not found.");

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var roleResult = await _userManager.AddToRoleAsync(user, assignRoleDto.Role);

            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);

            return Ok("Role assigned successfully.");
        }
    }
}