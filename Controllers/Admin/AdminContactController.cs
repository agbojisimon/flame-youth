using System.Security.Claims;
using g_flame_youth.DTOs.Contact;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("api/admin/contact")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        public AdminContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts([FromQuery] ContactQueryObject query)
        {
            var contacts = await _contactService.GetContactsAsync(query);

            return Ok(new ApiResponse<List<ContactResponseDto>>
            {
                isSuccess = true,
                Message = contacts.Count == 0
                ? "No contact is available at the moment"
                : "Contacts retrieved successfully",
                Data = contacts
            });
        }

        [HttpGet("{Id:int}")]
        public async Task<IActionResult> GetContactByIdAsync([FromRoute] int Id)
        {
            var contact = await _contactService.GetContactByIdAsync(Id);

            if (contact == null)
                return NotFound($"Contacts with ID {Id} is not found");

            return Ok(new ApiResponse<ContactResponseDto?>
            {
                isSuccess = true,
                Message = "Contacts retrieved successfully",
                Data = contact
            });
        }

        [HttpDelete("{Id:int}")]
        public async Task<IActionResult> DeleteContactAsync([FromRoute] int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID not found");

            var isDeleted = await _contactService.DeleteContactAsync(Id);

            if (!isDeleted)
                return NotFound($"Prayer request with ID {Id} not found.");

            return Ok(new ApiResponse<string>
            {
                isSuccess = true,
                Message = "Contact request deleted successfully",
                Data = $"Contact request with ID {Id} has been deleted."
            });
        }
    }
}
