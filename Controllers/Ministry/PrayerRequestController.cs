using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Ministry
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

            string name;
            string email;
            string? appUserId = null;

            if (isLoggedIn)
            {
                // Pull from JWT — the user already authenticated, trust the token
                appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var firstName = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;
                var lastName = User.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;
                name = $"{firstName} {lastName}".Trim();
                email = User.FindFirstValue(ClaimTypes.Email) ?? createDto.Email;
            }
            else
            {
                // Not logged in — use what they provided in the DTO
                // (both Name and Email are now [Required] on the DTO, so they'll
                //  never be null/empty here if model validation passed)
                name = createDto.Name;
                email = createDto.Email;
            }

            var result = await _prayerService.CreateAsync(createDto, name, email, appUserId);

            return CreatedAtAction("GetById", "AdminPrayerRequest", new { id = result.Id }, new
            {
                isSuccess = true,
                message = "Your prayer request has been received. " +
                          "Our pastoral team will be in touch within 48 hours.",
                data = result
            });
        }
    }
}