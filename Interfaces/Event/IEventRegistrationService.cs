using GlobalFlameMinistry.API.DTOs.Event;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IEventRegistrationService
    {
        Task<EventRegistrationResponseDto> RegisterAsync(
            int eventId,
            RegisterForEventDto dto,
            string eventTitle,
            DateTime startDate,
            DateTime endDate,
            string location,
            string? appUserId = null);

        Task<List<EventRegistrationResponseDto>> GetByEventIdAsync(int eventId);
        Task<int> GetCountByEventIdAsync(int eventId);
    }
}