using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/announcements")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IAnnouncementService announceService;
        public AnnouncementController(IAnnouncementService announceService)
        {
            this.announceService = announceService;
        }

        //GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] AnnouncementQueryObject query)
        {
            query.Module = "Ministry";
            query.IsPublished = true;

            var result = await announceService.GetAllAsync(query);

            return Ok(result);
        }

        // GET by id
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var announcement = await announceService.GetByIdAsync(id);

            if (announcement is null)
                return NotFound("Announcement not found");

            return Ok(announcement);
        }

        // GET by slug
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var announcement = await announceService.GetBySlugAsync(slug);
            if (announcement is null) return NotFound("Announcement not found");
            return Ok(announcement);
        }
    }
}