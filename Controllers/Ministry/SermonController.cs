using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/sermons")]
    [ApiController]
    public class SermonController : ControllerBase
    {
        private readonly ISermonService _service;

        public SermonController(ISermonService service)
        {
            _service = service;
        }

        // GET /api/ministry/sermons
        // Public — published sermons only
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SermonQueryObject query)
        {
            var result = await _service.GetPublishedAsync(query);
            return Ok(result);
        }

        // GET /api/ministry/sermons/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sermon = await _service.GetByIdAsync(id);
            if (sermon is null) return NotFound("Sermon not found");
            if (!sermon.IsPublished) return NotFound("Sermon not found");
            return Ok(sermon);
        }
    }
}