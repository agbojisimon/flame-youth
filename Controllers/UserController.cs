using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.User;
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
        private readonly UserManager<AppUser> _userManager;
        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userManager.Users.ToListAsync();

            var userDto = users.Select(u => u.ToUserDto());

            return Ok(userDto);
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
    }
}