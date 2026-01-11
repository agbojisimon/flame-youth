using System.Security.Claims;
using g_flame_youth.DTOs.PrayerRequest;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
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

        [HttpPost("send-prayer-request")]
        public async Task<IActionResult> CreatePrayer([FromBody] CreatePrayerDto createPrayerDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID not found");

            var createdPrayer = await _prayerService.CreatePrayerAsync(createPrayerDto);

            return StatusCode(StatusCodes.Status201Created,
                new ApiResponse<PrayerRequestResponseDto?>
                {
                    isSuccess = true,
                    Message = "Prayer request sent successfully",
                    Data = createdPrayer
                });
        }
    }
}