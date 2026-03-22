using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IEventRegistrationRepository
    {
        Task<EventRegistration> CreateAsync(EventRegistration registration);
        Task<List<EventRegistration>> GetByEventIdAsync(int eventId);
        Task<int> GetCountByEventIdAsync(int eventId);
        Task<bool> IsAlreadyRegisteredAsync(int eventId, string email);
    }
}