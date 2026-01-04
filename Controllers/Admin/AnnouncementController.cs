using System.Security.Claims;
using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("g-flame-youth/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService _announceService;
        public AnnouncementController(IAnnouncementService announceService)
        {
            _announceService = announceService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAnnouncements([FromQuery] AnnouncementQueryObject query)
        {
            var announcements = await _announceService.GetAnnouncementsAsync(query);

            return Ok(announcements);
        }

        [HttpGet("{Id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAnnouncementById([FromRoute] int Id)
        {
            var announcement = await _announceService.GetAnnouncementByIdAsync(Id);

            if (announcement == null)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(announcement);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var createdAnnouncement = await _announceService.CreateAnnouncementAsync(createDto, userId);

            return CreatedAtAction(nameof(GetAnnouncementById), new { id = createdAnnouncement.Id },
                createdAnnouncement
            );
        }

        [HttpPut("{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAnnouncement([FromRoute] int Id, [FromBody] UpdateAnnouncementDto updateAnnouncementDto, string userId)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var updatedAnnouncement = await _announceService.UpdateAnnouncementAsync(Id, updateAnnouncementDto, UserId);

            if (updatedAnnouncement == null)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(updatedAnnouncement);
        }

        [HttpDelete("{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnnouncement([FromRoute] int Id, string userId)
        {
            var UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isDeleted = await _announceService.DeleteAnnouncementAsync(Id, UserId);

            if (!isDeleted)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(new { message = "Announcement deleted successfully." });
        }
    }
}