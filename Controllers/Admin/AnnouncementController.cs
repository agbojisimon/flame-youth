using System.Security.Claims;
using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("api/[controller]")]
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

            return Ok(new ApiResponse<List<AnnouncementDto>>
            {
                isSuccess = true,
                Message = "Announcements Retrieved Successfully",
                Data = announcements
            });
        }

        [HttpGet("{Id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAnnouncementById([FromRoute] int Id)
        {
            var announcement = await _announceService.GetAnnouncementByIdAsync(Id);

            if (announcement == null)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(new ApiResponse<AnnouncementDto?>
            {
                isSuccess = true,
                Message = "Announcement Retrieved Successfully",
                Data = announcement
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateAnnouncement([FromBody] CreateAnnouncementDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new ApiResponse<string?>
                {
                    isSuccess = false,
                    Message = "User ID not found.",
                    Data = null
                });

            var createdAnnouncement = await _announceService.CreateAnnouncementAsync(createDto, userId);

            return CreatedAtAction(nameof(GetAnnouncementById), new { id = createdAnnouncement.Id },
                new ApiResponse<AnnouncementDto>
                {
                    isSuccess = true,
                    Message = "Announcement created successfully.",
                    Data = createdAnnouncement
                }
            );
        }

        [HttpPut("{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateAnnouncement([FromRoute] int Id, [FromBody] UpdateAnnouncementDto updateAnnouncementDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID must not be empty");

            var updatedAnnouncement = await _announceService.UpdateAnnouncementAsync(Id, updateAnnouncementDto);

            if (updatedAnnouncement == null)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(new ApiResponse<AnnouncementDto>
            {
                isSuccess = true,
                Message = "Announcement updated successfully.",
                Data = updatedAnnouncement
            });
        }

        [HttpDelete("{Id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAnnouncement([FromRoute] int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID must not be found");

            var isDeleted = await _announceService.DeleteAnnouncementAsync(Id);

            if (!isDeleted)
                return NotFound($"Announcement with ID {Id} not found.");

            return Ok(new { message = "Announcement deleted successfully." });
        }
    }
}