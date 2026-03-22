using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Youth
{
    [Route("api/youth/announcements")]
    [ApiController]
    public class YouthAnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announceService;
        public YouthAnnouncementController(IAnnouncementService announceService)
        {
            _announceService = announceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AnnouncementQueryObject query)
        {
            // Lock module to Youth — user cannot override this via query string
            query.Module = "Youth";
            query.IsPublished = true;

            var result = await _announceService.GetAllAsync(query);

            return Ok(result);
        }

        // GET /api/youth/announcements/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var announcement = await _announceService.GetByIdAsync(id);

            if (announcement is null)
                return NotFound("Announcement not found");

            // Guard — prevents someone using this route to read Ministry announcements
            if (announcement.Module != "Youth")
                return NotFound("Announcement not found");

            return Ok(announcement);
        }

        // POST /api/youth/announcements
        // Admin only — only admin can create Youth announcements
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto createDto)
        {
            createDto.Module = "Youth";

            var createdById = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(createdById))
                return Unauthorized("Unauthorized");

            var announcement = await _announceService.CreateAsync(createDto, createdById);

            return CreatedAtAction(nameof(GetById), new { id = announcement.Id }, announcement);
        }

        // PUT /api/youth/announcements/5
        // Admin only
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAnnouncementDto dto)
        {
            // Verify it belongs to Youth before allowing update
            var existing = await _announceService.GetByIdAsync(id);

            if (existing is null)
                return NotFound("Announcement not found");

            if (existing.Module != "Youth")
                return NotFound("Announcement not found");

            var announcement = await _announceService.UpdateAsync(id, dto);

            return Ok(announcement);
        }

        // DELETE /api/youth/announcements/5
        // Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            // Verify it belongs to Youth before allowing delete
            var existing = await _announceService.GetByIdAsync(id);

            if (existing is null)
                return NotFound("Announcement not found");

            if (existing.Module != "Youth")
                return NotFound("Announcement not found");

            var deleted = await _announceService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Announcement not found");

            return Ok("Youth announcement deleted successfully");
        }
    }
}