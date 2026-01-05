using g_flame_youth.Helpers;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface IEvenRepository
    {
        Task<List<Event>> GetEventsAsync(EventQueryObject query);
        Task<Event?> GetEventByIdAsync(int Id);
        Task CreateEventAsync(Event newEvent);
        Task UpdateEventAsync(Event updatedEvent);
        Task<bool> DeleteEventAsync(int Id);
    }
}