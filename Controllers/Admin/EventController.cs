using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using g_flame_youth.DTOs.Event;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace g_flame_youth.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("get-all-events")]
        public async Task<IActionResult> GetEvent([FromQuery] EventQueryObject query)
        {
            var events = await _eventService.GetEventsAsync(query);

            return Ok(new ApiResponse<List<EventResponseDto>>
            {
                isSuccess = true,
                Message = "Events retrieved successfully",
                Data = events,
            });
        }

        [HttpGet("{Id:int}/get-event")]
        public async Task<IActionResult> GetEventById([FromRoute] int Id)
        {
            var events = await _eventService.GetEventByIdAsync(Id);

            if (events == null)
                return NotFound($"Event with ID {Id} is not found");

            return Ok(new ApiResponse<EventResponseDto?>
            {
                isSuccess = true,
                Message = "Event retrieved successfully",
                Data = events
            });
        }

        [HttpPost("create-event")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID not found");

            var createdEvent = await _eventService.CreateEventAsync(createDto);

            return CreatedAtAction(nameof(GetEventById), new { Id = createdEvent.Id },
            new ApiResponse<EventResponseDto>
            {
                isSuccess = true,
                Message = "Event created successfully",
                Data = createdEvent
            });
        }

        [HttpPut("{Id:int}/update-event")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEvent([FromRoute] int Id, [FromBody] UpdateEventDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID not found");

            var updatedEvent = await _eventService.UpdateEventAsync(Id, updateDto);

            if (updatedEvent == null)
                return NotFound($"Event with ID {Id} not found.");

            return Ok(new ApiResponse<EventResponseDto?>
            {
                isSuccess = true,
                Message = "Event updated successfully",
                Data = updatedEvent
            });
        }

        [HttpDelete("{Id:int}/delete-event")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEvent([FromRoute] int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return NotFound("User ID not found");

            var isDeleted = await _eventService.DeleteEventAsync(Id);

            if (!isDeleted)
                return NotFound($"Event with ID {Id} not found.");

            return Ok(new ApiResponse<string>
            {
                isSuccess = true,
                Message = "Event deleted successfully",
                Data = $"Event with ID {Id} has been deleted."
            });
        }
    }
}