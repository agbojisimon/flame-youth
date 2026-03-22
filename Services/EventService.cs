using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        public EventService(IEventRepository eventRepo)
        {
            _eventRepo = eventRepo;
        }

        public async Task<EventResponseDto> CreateAsync(CreateEventDto createDto)
        {
            var evt = createDto.ToModel();

            var created = await _eventRepo.CreateAsync(evt);

            return created.ToEventResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _eventRepo.DeleteAsync(id);
        }

        public async Task<PagedResult<EventResponseDto>> GetAllAsync(EventQueryObject query)
        {
            var events = await _eventRepo.GetAllAsync(query);
            var totalCount = await _eventRepo.GetCountAsync(query);

            return new PagedResult<EventResponseDto>
            {
                Items = events.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            var evt = await _eventRepo.GetByIdAsync(id);

            if (evt is null)
                return null;

            return evt.ToEventResponseDto();
        }

        public async Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto updateDto)
        {
            var updated = await _eventRepo.UpdateAsync(id, updateDto);

            if (updated is null)
                return null;

            return updated.ToEventResponseDto();
        }
    }
}