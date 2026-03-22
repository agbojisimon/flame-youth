using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GlobalFlameMinistry.API.Models;
using GlobalFlameMinistry.API.DTOs.Testimony;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/testimonies")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminTestimonyController : ControllerBase
    {
        private readonly ITestimonyService _testimonyService;
        public AdminTestimonyController(ITestimonyService testimonyService)
        {
            _testimonyService = testimonyService;
        }

        [HttpGet]
        public async Task<IActionResult> GetApproved([FromQuery] TestimonyQueryObject query)
        {
            var result = await _testimonyService.GetApprovedAsync(query);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var testimony = await _testimonyService.GetByIdAsync(id);

            if (testimony is null)
                return NotFound("Testimony not found");

            if (testimony.Status != TestimonyStatus.Approved.ToString())
                return NotFound("Testimony not found");

            return Ok(testimony);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTestimonyDto updateDto)
        {
            var result = await _testimonyService.UpdateStatusAsync(id, updateDto);

            if (result is null)
                return NotFound("Testimony not found");

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _testimonyService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Testimony not found");

            return Ok("Testimony deleted successfully");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll([FromQuery] TestimonyQueryObject query)
        {
            var result = await _testimonyService.GetAllAsync(query);

            return Ok(result);
        }
    }
}