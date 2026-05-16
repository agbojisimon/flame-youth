using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Members
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestimonyController : ControllerBase
    {
        private readonly ITestimonyService _testimonyService;
        public TestimonyController(ITestimonyService testimonyService)
        {
            _testimonyService = testimonyService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestimonyDto createDto)
        {
            var isLoggedIn = User.Identity?.IsAuthenticated ?? false;

            string? name = null;
            string? appUserId = null;

            if (isLoggedIn)
            {
                // Pull from JWT
                appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var firstName = User.FindFirstValue(ClaimTypes.GivenName);
                var lastName = User.FindFirstValue(ClaimTypes.Surname);
                name = $"{firstName} {lastName}".Trim();
            }
            else
            {
                // Anonymous
                name = createDto.FullName;
            }

            var result = await _testimonyService.CreateAsync(createDto, name, appUserId);
            return CreatedAtAction("GetById", "AdminTestimony", new { id = result.Id }, result);
        }
    }
}