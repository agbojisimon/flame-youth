using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        private readonly HybridCache _cache;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepo, HybridCache cache, ILogger<EventService> logger)
        {
            _eventRepo = eventRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<EventResponseDto> CreateAsync(CreateEventDto createDto)
        {
            var evt = createDto.ToModel();
            var created = await _eventRepo.CreateAsync(evt);

            try
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagEvents, CancellationToken.None);
                _logger.LogInformation("[EventService] Created event ID {Id}, invalidated cache tag {Tag}", created.Id, CacheKeys.TagEvents);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[EventService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagEvents);
            }

            return created.ToEventResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _eventRepo.DeleteAsync(id);

            if (result)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagEvents, CancellationToken.None);
                    _logger.LogInformation("[EventService] Deleted event ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagEvents);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[EventService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagEvents);
                }
            }

            return result;
        }

        public async Task<PagedResult<EventResponseDto>> GetAllAsync(EventQueryObject query)
        {
            var cacheKey = $"events:p{query.PageNumber}:s{query.PageSize}:upcoming{query.UpcomingOnly}:ongoing{query.OngoingOnly}:past{query.PastOnly}:cancelled{query.IsCancelled}:module{query.Module}";

            async ValueTask<PagedResult<EventResponseDto>> Factory(CancellationToken ct)
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

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagEvents], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.EventId, id);

            async ValueTask<EventResponseDto?> Factory(CancellationToken ct)
            {
                var evt = await _eventRepo.GetByIdAsync(id);
                return evt?.ToEventResponseDto();
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagEvents], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<EventResponseDto?> GetBySlugAsync(string slug)
        {
            var cacheKey = string.Format(CacheKeys.EventSlug, slug);

            async ValueTask<EventResponseDto?> Factory(CancellationToken ct)
            {
                var evt = await _eventRepo.GetBySlugAsync(slug);
                return evt?.ToEventResponseDto();
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagEvents], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto updateDto)
        {
            var updated = await _eventRepo.UpdateAsync(id, updateDto);

            if (updated is not null)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagEvents, CancellationToken.None);
                    _logger.LogInformation("[EventService] Updated event ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagEvents);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[EventService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagEvents);
                }
            }

            return updated?.ToEventResponseDto();
        }
    }
}
