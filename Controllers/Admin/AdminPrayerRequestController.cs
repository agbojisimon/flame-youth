using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
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

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PrayerRequestQueryObject query)
        {
            var result = await _prayerService.GetAllAsync(query);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var request = await _prayerService.GetByIdAsync(id);

            if (request is null)
                return NotFound("Prayer request not found");

            return Ok(request);
        }

        [HttpPatch("{id:int}/attend")]
        public async Task<IActionResult> MarkAsAttended(int id, [FromBody] UpdatePrayerRequestDto dto)
        {
            var result = await _prayerService.MarkAsAttendedAsync(id, dto);

            if (result is null)
                return NotFound("Prayer request not found");

            return Ok(result);
        }

        [HttpDelete("{id:int}/permanent")]
        public async Task<IActionResult> HardDelete(int id)
        {
            var existing = await _prayerService.GetByIdAsync(id);

            if (existing is null)
                return NotFound("Prayer request not found");

            if (!existing.IsAttendedTo)
                return BadRequest(
                    "Only attended prayer requests can be permanently deleted.");

            var deleted = await _prayerService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Prayer request not found");

            return Ok("Prayer request permanently deleted");
        }
    }
}