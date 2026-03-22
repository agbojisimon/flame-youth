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
            string location);

        Task<List<EventRegistrationResponseDto>> GetByEventIdAsync(int eventId);
        Task<int> GetCountByEventIdAsync(int eventId);
    }
}