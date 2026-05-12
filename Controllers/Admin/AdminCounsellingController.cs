using GlobalFlameMinistry.API.DTOs.Counselling;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Counselling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/counselling")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminCounsellingController : ControllerBase
    {
        private readonly ICounsellingService _service;

        public AdminCounsellingController(ICounsellingService service)
        {
            _service = service;
        }

        // GET /api/admin/counselling
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] CounsellingQueryObject query)
        {
            var result = await _service.GetAllAsync(query);
            return Ok(result);
        }

        // GET /api/admin/counselling/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result is null)
                return NotFound("Counselling request not found");

            return Ok(result);
        }

        // PUT /api/admin/counselling/5/status
        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(
            int id, [FromBody] UpdateCounsellingStatusDto dto)
        {
            var result = await _service.UpdateStatusAsync(id, dto.Status);

            if (result is null)
                return NotFound("Counselling request not found");

            return Ok(result);
        }

        // DELETE /api/admin/counselling/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Counselling request not found");

            return StatusCode(204);
        }
    }
}