using System.Security.Claims;
using g_flame_youth.DTOs.PrayerRequest;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("api/admin/prayer-requests")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPrayerRequestController : ControllerBase
    {
        private readonly IPrayerRequestService _prayerService;
        public AdminPrayerRequestController(IPrayerRequestService prayerService)
        {
            _prayerService = prayerService;
        }

        [HttpGet("prayer-requests")]
        public async Task<IActionResult> GetAllPrayerRequest([FromQuery] PrayerReqeustQueryObject query)
        {
            var prayers = await _prayerService.GetPrayerRequestsAsync(query);

            return Ok(new ApiResponse<List<PrayerRequestResponseDto>>
            {
                isSuccess = true,
                Message = prayers.Count == 0
                ? "No prayer request is available at the moment"
                : "Prayer requests retrieved successfully",
                Data = prayers,
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetPrayerById([FromRoute] int id)
        {
            var prayer = await _prayerService.GetByIdAsync(id);

            if (prayer == null)
                return NotFound($"Prayer request with ID {id} is not found");

            return Ok(new ApiResponse<PrayerRequestResponseDto?>
            {
                isSuccess = true,
                Message = "Prayer request retrieved successfully",
                Data = prayer
            });
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePrayer([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID not found");

            var isDeleted = await _prayerService.DeletePrayerAsync(id);

            if (!isDeleted)
                return NotFound($"Prayer request with ID {id} not found.");

            return Ok(new ApiResponse<string>
            {
                isSuccess = true,
                Message = "Prayer request deleted successfully",
                Data = $"Prayer request with ID {id} has been deleted."
            });
        }
    }
}