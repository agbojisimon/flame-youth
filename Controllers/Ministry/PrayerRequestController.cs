using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PrayerRequestController : ControllerBase
    {
        private readonly IPrayerRequestService _prayerService;
        public PrayerRequestController(IPrayerRequestService prayerService)
        {
            _prayerService = prayerService;
        }

        [HttpGet("track/{token}")]
        public async Task<IActionResult> TrackByToken(string token)
        {
            var request = await _prayerService.GetByTokenAsync(token);

            if (request is null)
                return NotFound("Prayer request not found");

            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePrayerDto createDto)
        {
            var isLoggedIn = User.Identity?.IsAuthenticated ?? false;

            string? name = null;
            string? email = null;
            string? appUserId = null;

            if (isLoggedIn)
            {
                // Pull everything from JWT
                appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                name = User.FindFirstValue(ClaimTypes.GivenName) + " " +
                       User.FindFirstValue(ClaimTypes.Surname);
                email = User.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                // Anonymous — use whatever they chose to provide, or nothing at all
                name = createDto.Name;
                email = createDto.Email;
            }

            var result = await _prayerService.CreateAsync(createDto, name?.Trim(), email, appUserId);

            return Ok(result);
        }
    }
}