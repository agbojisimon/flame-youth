using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace GlobalFlameMinistry.API.Controllers.Ministry
{
    [Route("api/ministry/events")]
    [ApiController]
    [EnableRateLimiting("GeneralPolicy")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _service;
        private readonly IEventRegistrationService _registrationService;

        public EventController(
            IEventService service,
            IEventRegistrationService registrationService)
        {
            _service = service;
            _registrationService = registrationService;
        }

        // GET /api/ministry/events
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EventQueryObject query)
        {
            query.IsCancelled = false;
            var result = await _service.GetAllAsync(query);
            return Ok(result);
        }

        // GET /api/ministry/events/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt is null) return NotFound("Event not found");
            return Ok(evt);
        }

        // GET /api/ministry/events/{slug}
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var evt = await _service.GetBySlugAsync(slug);
            if (evt is null) return NotFound("Event not found");
            return Ok(evt);
        }

        // POST /api/ministry/events/{id}/register
        [HttpPost("{id:int}/register")]
        public async Task<IActionResult> Register(int id, [FromBody] RegisterForEventDto dto)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt is null)
                return NotFound("Event not found");

            if (evt.IsCancelled)
                return BadRequest("This event has been cancelled.");

            if (!evt.AcceptsRegistrations)
                return BadRequest("This event is not accepting registrations.");

            // If authenticated, link the registration to the user's account
            var appUserId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            // For logged-in users, use the JWT email so they
            // cannot register another person's email
            if (appUserId is not null)
                dto.Email = User.FindFirstValue(ClaimTypes.Email) ?? dto.Email;

            try
            {
                var registration = await _registrationService.RegisterAsync(
                    id,
                    dto,
                    evt.Title,
                    evt.StartDate,
                    evt.EndDate,
                    evt.Location,
                    appUserId
                );

                return Ok(new
                {
                    message = $"Registration confirmed! A confirmation email has been sent to {dto.Email}.",
                    data = registration
                });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}