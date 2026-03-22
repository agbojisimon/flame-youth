using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllAsync(EventQueryObject query);
        Task<int> GetCountAsync(EventQueryObject query);
        Task<Event?> GetByIdAsync(int id);
        Task<Event> CreateAsync(Event evt);
        Task<Event?> UpdateAsync(int id, UpdateEventDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}