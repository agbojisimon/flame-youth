using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [HttpGet("{Id:string}")]
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
    }
}