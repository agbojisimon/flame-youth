using g_flame_youth.DTOs.Event;
using g_flame_youth.Models;

namespace g_flame_youth.Mappers
{
    public static class EventMapper
    {
        public static EventResponseDto ToEventResponseDto(this Event eventModel)
        {
            return new EventResponseDto
            {
                Id = eventModel.Id,
                Title = eventModel.Title,
                Description = eventModel.Description,
                StartDate = eventModel.StartDate,
                EndDate = eventModel.EndDate,
                Location = eventModel.Location,
                ImageUrl = eventModel.ImageUrl,
                CreatedOn = eventModel.CreatedOn
            };
        }

        public static Event ToEventFromCreateDto(this CreateEventDto createEventDto)
        {
            return new Event
            {
                Title = createEventDto.Title,
                Description = createEventDto.Description,
                StartDate = createEventDto.StartDate,
                EndDate = createEventDto.EndDate,
                Location = createEventDto.Location,
                ImageUrl = createEventDto.ImageUrl,
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}