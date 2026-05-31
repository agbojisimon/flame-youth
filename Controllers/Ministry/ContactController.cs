using GlobalFlameMinistry.API.DTOs.Contact;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/contacts")]
    [ApiController]
    [AllowAnonymous]
    [EnableRateLimiting("GeneralPolicy")]
    public class ContactController : ControllerBase
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateContactDto dto)
        {
            var result = await _contactService.CreateAsync(dto);
            return CreatedAtAction("GetById", "AdminContact", new { id = result.Id }, result);
        }
    }
}