using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Ministry;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/ministries")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminMinistryController : ControllerBase
    {
        private readonly IMinistryService _service;

        public AdminMinistryController(IMinistryService service)
        {
            _service = service;
        }

        // GET /api/admin/ministries
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] MinistryQueryObject query)
        {
            var result = await _service.GetAllAsync(query);

            return Ok(result);
        }

        // GET /api/admin/ministries/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ministry = await _service.GetByIdAsync(id);

            if (ministry is null)
                return NotFound("Ministry not found");

            return Ok(ministry);
        }

        // POST /api/admin/ministries
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMinistryDto dto)
        {
            var ministry = await _service.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = ministry.Id }, ministry);
        }

        // PUT /api/admin/ministries/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id, [FromBody] UpdateMinistryDto dto)
        {
            var ministry = await _service.UpdateAsync(id, dto);

            if (ministry is null)
                return NotFound("Ministry not found");

            return Ok(ministry);
        }

        // DELETE /api/admin/ministries/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Ministry not found");

            return NoContent();
        }
    }
}