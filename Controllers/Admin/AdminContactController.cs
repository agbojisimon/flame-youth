using GlobalFlameMinistry.API.DTOs.Contact;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/contacts")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        public AdminContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        // GET /api/ministry/contacts
        // Admin only — private messages sent to the church
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ContactQueryObject query)
        {
            var result = await _contactService.GetAllAsync(query);
            return Ok(result);
        }

        // Admin only
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var contact = await _contactService.GetByIdAsync(id);

            if (contact is null)
                return NotFound("Contact message not found");

            return Ok(contact);
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateContactDto updateDto)
        {
            var result = await _contactService.UpdateStatusAsync(id, updateDto);

            if (result is null)
                return NotFound("Contact message not found");

            return Ok(result);
        }

        // Admin only
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _contactService.DeleteAsync(id);

            if (!deleted)
                return NotFound("Contact message not found");

            return NoContent();
        }
    }
}
