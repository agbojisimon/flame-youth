
using g_flame_youth.DTOs.Event;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;

namespace g_flame_youth.Services
{
    public class EventService : IEventService
    {
        private readonly IEvenRepository _eventRepo;
        public EventService(IEvenRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }
        public async Task<EventResponseDto> CreateEventAsync(CreateEventDto createDto)
        {
            var events = createDto.ToEventFromCreateDto();
            events.CreatedOn = DateTime.UtcNow;

            await _eventRepo.CreateEventAsync(events);

            return events.ToEventResponseDto();
        }

        public async Task<bool> DeleteEventAsync(int Id)
        {
            var events = await _eventRepo.GetEventByIdAsync(Id);

            if (events == null)
                return false;

            return await _eventRepo.DeleteEventAsync(Id);
        }

        public async Task<EventResponseDto?> GetEventByIdAsync(int Id)
        {
            var events = await _eventRepo.GetEventByIdAsync(Id);

            if (events == null)
                return null;

            return events.ToEventResponseDto();
        }

        public async Task<List<EventResponseDto>> GetEventsAsync(EventQueryObject query)
        {
            var events = await _eventRepo.GetEventsAsync(query);

            return events.Select(e => e.ToEventResponseDto()).ToList();
        }

        public async Task<EventResponseDto?> UpdateEventAsync(int Id, UpdateEventDto updateDto)
        {
            var events = await _eventRepo.GetEventByIdAsync(Id);

            if (events == null)
                return null;

            events.Title = updateDto.Title;
            events.Description = updateDto.Description;
            events.StartDate = updateDto.StartDate;
            events.EndDate = updateDto.EndDate;
            events.ImageUrl = updateDto.ImageUrl;
            events.UpdatedOn = DateTime.UtcNow;

            await _eventRepo.UpdateEventAsync(events);
            return events.ToEventResponseDto();
        }
    }
}