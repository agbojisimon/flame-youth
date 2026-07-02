using GlobalFlameMinistry.API.DTOs.BulkEmail;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.BulkEmail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/bulk-email")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminBulkEmailController : ControllerBase
    {
        private readonly IBulkEmailService _service;

        public AdminBulkEmailController(IBulkEmailService service)
        {
            _service = service;
        }

        // GET /api/admin/bulk-email/history
        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] BulkEmailQueryObject query)
        {
            var result = await _service.GetHistoryAsync(query);
            return Ok(result);
        }

        // GET /api/admin/bulk-email/stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            var result = await _service.GetStatsAsync();
            return Ok(result);
        }

        // POST /api/admin/bulk-email/send
        [HttpPost("send")]
        [EnableRateLimiting("BulkEmailPolicy")]
        public async Task<IActionResult> SendNow([FromBody] SendBulkEmailDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            var name = User.FindFirstValue(ClaimTypes.GivenName) ?? "Admin";

            var result = await _service.SendNowAsync(dto, userId, name);

            return Ok(result);
        }

        // POST /api/admin/bulk-email/schedule
        [HttpPost("schedule")]
        [EnableRateLimiting("BulkEmailPolicy")]
        public async Task<IActionResult> Schedule([FromBody] SendBulkEmailDto dto)
        {
            if (dto.ScheduledAt is null || dto.ScheduledAt <= DateTime.UtcNow)
                return BadRequest("ScheduledAt must be a future date and time.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";

            var name = User.FindFirstValue(ClaimTypes.GivenName) ?? "Admin";

            var result = await _service.ScheduleAsync(dto, userId, name);

            return Ok(result);
        }

        // DELETE /api/admin/bulk-email/{id}
        [HttpDelete("{id:int}")]
        [EnableRateLimiting("BulkEmailPolicy")]
        public async Task<IActionResult> Cancel(int id)
        {
            var cancelled = await _service.CancelScheduledAsync(id);

            if (!cancelled)
                return NotFound("Scheduled email not found or already sent.");

            return Ok("Scheduled email cancelled.");
        }
    }
}