using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IEventService
    {
        Task<PagedResult<EventResponseDto>> GetAllAsync(EventQueryObject query);
        Task<EventResponseDto?> GetByIdAsync(int id);
        Task<EventResponseDto> CreateAsync(CreateEventDto dto);
        Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto dto);
        Task<bool> DeleteAsync(int id);
    }
}