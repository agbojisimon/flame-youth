using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.Counselling;
using GlobalFlameMinistry.API.Interfaces.Counselling;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/counselling")]
    [ApiController]
    [AllowAnonymous]
    [EnableRateLimiting("GeneralPolicy")]
    public class CounsellingController : ControllerBase
    {
        private readonly ICounsellingService _service;

        public CounsellingController(ICounsellingService service)
        {
            _service = service;
        }

        // POST /api/ministry/counselling
        [HttpPost]
        public async Task<IActionResult> Submit(
            [FromBody] CreateCounsellingRequestDto dto)
        {
            // Attach user ID if authenticated — otherwise null (guest submission)
            var appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _service.CreateAsync(dto, appUserId);

            return CreatedAtAction("GetById", "AdminCounselling", new { id = result.Id }, new
            {
                isSuccess = true,
                message = "Your counselling request has been received. " +
                          "A member of our pastoral team will contact you shortly.",
                data = result
            });
        }
    }
}