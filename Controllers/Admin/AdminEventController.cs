using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GlobalFlameMinistry.API.Controllers.Admin
{
    [Route("api/admin/events")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminEventController : ControllerBase
    {
        private readonly IEventService _service;
        private readonly IEventRegistrationService _registrationService;

        public AdminEventController(
            IEventService service,
            IEventRegistrationService registrationService)
        {
            _service = service;
            _registrationService = registrationService;
        }

        // GET /api/admin/events
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EventQueryObject query)
        {
            var result = await _service.GetAllAsync(query);
            return Ok(result);
        }

        // GET /api/admin/events/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _service.GetByIdAsync(id);
            if (evt is null) return NotFound("Event not found");
            return Ok(evt);
        }

        // GET /api/admin/events/5/registrations
        // Admin sees who registered for an event
        [HttpGet("{id:int}/registrations")]
        public async Task<IActionResult> GetRegistrations(int id)
        {
            var registrations = await _registrationService.GetByEventIdAsync(id);
            var count = await _registrationService.GetCountByEventIdAsync(id);
            return Ok(new { count, registrations });
        }

        // POST /api/admin/events
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            var evt = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = evt.Id }, evt);
        }

        // PUT /api/admin/events/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            var evt = await _service.UpdateAsync(id, dto);
            if (evt is null)
                return NotFound("Event not found");

            return Ok(evt);
        }

        // DELETE /api/admin/events/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
                return NotFound("Event not found");

            return Ok("Event deleted successfully");
        }
    }
}