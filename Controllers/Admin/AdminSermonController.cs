using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/sermons")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminSermonController : ControllerBase
    {
        private readonly ISermonService _service;

        public AdminSermonController(ISermonService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SermonQueryObject query)
        {
            var result = await _service.GetAllAsync(query);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sermon = await _service.GetByIdAsync(id);

            if (sermon is null)
                return NotFound("Sermon not found");

            return Ok(sermon);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSermonDto dto)
        {
            var sermon = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = sermon.Id }, sermon);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSermonDto dto)
        {
            var sermon = await _service.UpdateAsync(id, dto);

            if (sermon is null)
                return NotFound("Sermon not found");

            return Ok(sermon);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Sermon not found");

            return NoContent();
        }

        [HttpPut("{id}/toggle-featured")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var result = await _service.ToggleFeaturedAsync(id);

            if (result is null)
                return NotFound();

            return Ok(result);
        }
    }
}