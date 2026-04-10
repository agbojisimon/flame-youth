using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Ministry;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/ministries")]
    [ApiController]
    public class MinistryController : ControllerBase
    {
        private readonly IMinistryService _ministryService;
        private readonly IEventService _eventService;

        public MinistryController(IMinistryService ministryService, IEventService eventService)
        {
            _ministryService = ministryService;
            _eventService = eventService;
        }

        // GET /api/ministry/ministries
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] MinistryQueryObject query)
        {
            query.IsPublished = true;

            var result = await _ministryService.GetAllAsync(query);

            return Ok(result);
        }

        // GET /api/ministry/ministries/daughters-of-honour
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var ministry = await _ministryService.GetBySlugAsync(slug);

            if (ministry is null)
                return NotFound("Ministry not found");

            return Ok(ministry);
        }

        // GET /api/ministry/ministries/daughters-of-honour/events
        [HttpGet("{slug}/events")]
        public async Task<IActionResult> GetMinistryEvents(
            string slug,
            [FromQuery] EventQueryObject query)
        {
            var ministry = await _ministryService.GetBySlugAsync(slug);

            if (ministry is null)
                return NotFound("Ministry not found");

            query.MinistryId = ministry.Id;

            var result = await _eventService.GetAllAsync(query);

            return Ok(result);
        }
    }
}