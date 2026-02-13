using System.Security.Claims;
using g_flame_youth.DTOs.Contact;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Members
{
    [Route("api/member/contact")]
    [ApiController]
    [AllowAnonymous]
    public class MemberContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        public MemberContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContactAsync([FromBody] CreateContactDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID not found");

            var createdContact = await _contactService.CreateContactAsync(createDto);

            return Ok(createdContact);
        }
    }
}