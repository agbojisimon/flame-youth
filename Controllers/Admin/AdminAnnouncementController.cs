using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/announcements")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announceService;

        public AdminAnnouncementController(IAnnouncementService announceService)
        {
            _announceService = announceService;
        }

        // GET /api/admin/announcements
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AnnouncementQueryObject query)
        {
            var result = await _announceService.GetAllAsync(query);

            return Ok(result);
        }

        // GET /api/admin/announcements/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var announcement = await _announceService.GetByIdAsync(id);

            if (announcement is null)
                return NotFound("Announcement not found");

            return Ok(announcement);
        }

        // POST /api/admin/announcements
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAnnouncementDto dto)
        {
            var createdById = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(createdById))
                return Unauthorized("Unauthorized");

            var announcement = await _announceService.CreateAsync(dto, createdById);
            return CreatedAtAction(nameof(GetById), new { id = announcement.Id }, announcement);
        }

        // PUT /api/admin/announcements/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAnnouncementDto dto)
        {
            var announcement = await _announceService.UpdateAsync(id, dto);

            if (announcement is null)
                return NotFound("Announcement not found");

            return Ok(announcement);
        }

        // DELETE /api/admin/announcements/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _announceService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Announcement not found");

            return Ok("Announcement deleted successfully");
        }
    }
}