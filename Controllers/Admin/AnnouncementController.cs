using System.Security.Claims;
using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("g-flame-youth/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementRepository _announcementRepository;
        public AnnouncementController(IAnnouncementRepository announcement)
        {
            _announcementRepository = announcement;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAnnouncements([FromQuery] AnnouncementQueryObject query)
        {
            var announcements = await _announcementRepository.GetAnnouncementsAsync(query);

            var announcementDtos = announcements.Select(a => a.ToAnnouncementDto()).ToList();

            return Ok(announcementDtos);
        }

        [HttpGet("{Id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAnnouncementById([FromRoute] int Id)
        {
            var announcement = await _announcementRepository.GetAnnouncementByIdAsync(Id);
            if (announcement == null)
                return NotFound($"Announcement with ID {Id} not found.");

            var announcementDto = announcement.ToAnnouncementDto();

            return Ok(announcementDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto createAnnouncementDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var announcement = createAnnouncementDto.ToAnnouncementFromCreateDto(userId);

            var createdAnnouncement = await _announcementRepository.CreateAnnouncementAsync(announcement);

            return CreatedAtAction(nameof(GetAnnouncementById), new { id = createdAnnouncement.Id },
                createdAnnouncement.ToAnnouncementDto()
            );
        }

        [HttpPut("{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAnnouncement([FromRoute] int Id, [FromBody] UpdateAnnouncementDto updateAnnouncementDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var updatedAnnouncement = await _announcementRepository.UpdateAnnouncementAsync(Id, updateAnnouncementDto);

            if (updatedAnnouncement == null)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(updatedAnnouncement.ToAnnouncementDto());
        }

        [HttpDelete("{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnnouncement([FromRoute] int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isDeleted = await _announcementRepository.DeleteAnnouncementAsync(Id);

            if (!isDeleted)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(new { message = "Announcement deleted successfully." });
        }
    }
}