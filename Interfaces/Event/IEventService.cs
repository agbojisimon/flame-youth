using g_flame_youth.DTOs.Event;
using g_flame_youth.Helpers;

namespace g_flame_youth.Interfaces
{
    public interface IEventService
    {
        Task<List<EventResponseDto>> GetEventsAsync(EventQueryObject query);
        Task<EventResponseDto?> GetEventByIdAsync(int Id);
        Task<EventResponseDto> CreateEventAsync(CreateEventDto createDto);
        Task<EventResponseDto?> UpdateEventAsync(int Id, UpdateEventDto updateDto);
        Task<bool> DeleteEventAsync(int Id);
    }
}