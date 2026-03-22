using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Youth
{
    [Route("api/youth/events")]
    [ApiController]
    [Authorize]
    public class YouthEventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventRegistrationService _registrationService;

        public YouthEventController(
            IEventService eventService,
            IEventRegistrationService registrationService)
        {
            _eventService = eventService;
            _registrationService = registrationService;
        }

        // GET /api/youth/events
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EventQueryObject query)
        {
            query.Module = "Youth";
            query.IsCancelled = false;
            var result = await _eventService.GetAllAsync(query);
            return Ok(result);
        }

        // GET /api/youth/events/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _eventService.GetByIdAsync(id);
            if (evt is null) return NotFound("Event not found");
            if (evt.Module != "Youth") return NotFound("Event not found");
            return Ok(evt);
        }

        // POST /api/youth/events/{id}/register
        // Any logged in user can register for a youth event
        [HttpPost("{id:int}/register")]
        public async Task<IActionResult> Register(
            int id,
            [FromBody] RegisterForEventDto dto)
        {
            var evt = await _eventService.GetByIdAsync(id);
            if (evt is null) return NotFound("Event not found");
            if (evt.Module != "Youth") return NotFound("Event not found");
            if (evt.IsCancelled) return BadRequest("This event has been cancelled.");

            try
            {
                var registration = await _registrationService.RegisterAsync(
                    id,
                    dto,
                    evt.Title,
                    evt.StartDate,
                    evt.EndDate,
                    evt.Location
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

        // POST /api/youth/events — Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            dto.Module = "Youth";
            var evt = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = evt.Id }, evt);
        }

        // PUT /api/youth/events/5 — Admin only
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            var existing = await _eventService.GetByIdAsync(id);
            if (existing is null) return NotFound("Event not found");
            if (existing.Module != "Youth") return NotFound("Event not found");

            var evt = await _eventService.UpdateAsync(id, dto);
            return Ok(evt);
        }

        // DELETE /api/youth/events/5 — Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _eventService.GetByIdAsync(id);
            if (existing is null) return NotFound("Event not found");
            if (existing.Module != "Youth") return NotFound("Event not found");

            var deleted = await _eventService.DeleteAsync(id);
            if (!deleted) return NotFound("Event not found");
            return Ok("Youth event deleted successfully");
        }
    }
}