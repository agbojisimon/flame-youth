using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Event;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Memory;

namespace GlobalFlameMinistry.API.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<EventService> _logger;

        // Cache key constants
        private const string CACHE_KEY_UPCOMING_EVENTS = "events_upcoming";
        private const string CACHE_KEY_ALL_EVENTS = "events_all";
        private const string CACHE_KEY_EVENT_ID = "event_id_{0}";

        // Cache expiration times
        private readonly TimeSpan _listCacheExpiration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _itemCacheExpiration = TimeSpan.FromMinutes(10);

        public EventService(IEventRepository eventRepo, IMemoryCache memoryCache, ILogger<EventService> logger)
        {
            _eventRepo = eventRepo;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new event (admin endpoint). Invalidates event caches.
        /// </summary>
        public async Task<EventResponseDto> CreateAsync(CreateEventDto createDto)
        {
            var evt = createDto.ToModel();
            var created = await _eventRepo.CreateAsync(evt);

            // Invalidate event caches
            InvalidateEventCache();

            _logger.LogInformation("[EventService] Created new event with ID {Id}, invalidating caches", created.Id);

            return created.ToEventResponseDto();
        }

        /// <summary>
        /// Deletes an event (admin endpoint). Invalidates all event caches.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _eventRepo.DeleteAsync(id);

            if (result)
            {
                // Invalidate all event caches
                InvalidateEventCache();
                _logger.LogInformation("[EventService] Deleted event ID {Id}, invalidating caches", id);
            }

            return result;
        }

        /// <summary>
        /// Gets all events with caching. Cache expires after 5 minutes.
        /// Typically used for public-facing upcoming events.
        /// </summary>
        public async Task<PagedResult<EventResponseDto>> GetAllAsync(EventQueryObject query)
        {
            string cacheKey = CACHE_KEY_UPCOMING_EVENTS;

            // Attempt to retrieve from cache
            if (_memoryCache.TryGetValue(cacheKey, out PagedResult<EventResponseDto>? cachedResult))
            {
                _logger.LogInformation("[EventService] Cache hit for upcoming events");
                return cachedResult!;
            }

            _logger.LogInformation("[EventService] Cache miss for upcoming events - querying database");

            var events = await _eventRepo.GetAllAsync(query);
            var totalCount = await _eventRepo.GetCountAsync(query);

            var result = new PagedResult<EventResponseDto>
            {
                Items = events.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            // Store in cache with 5-minute expiration
            _memoryCache.Set(cacheKey, result, _listCacheExpiration);

            return result;
        }

        /// <summary>
        /// Gets an event by ID with caching. Cache expires after 10 minutes.
        /// </summary>
        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            string cacheKey = string.Format(CACHE_KEY_EVENT_ID, id);

            if (_memoryCache.TryGetValue(cacheKey, out EventResponseDto? cachedEvent))
            {
                _logger.LogInformation("[EventService] Cache hit for event ID {Id}", id);
                return cachedEvent;
            }

            _logger.LogInformation("[EventService] Cache miss for event ID {Id}", id);

            var evt = await _eventRepo.GetByIdAsync(id);

            if (evt is null)
                return null;

            var result = evt.ToEventResponseDto();
            _memoryCache.Set(cacheKey, result, _itemCacheExpiration);

            return result;
        }

        /// <summary>
        /// Updates an existing event (admin endpoint). Invalidates related event caches.
        /// </summary>
        public async Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto updateDto)
        {
            var updated = await _eventRepo.UpdateAsync(id, updateDto);

            if (updated is not null)
            {
                // Invalidate specific event caches
                InvalidateEventCache(id);
                _logger.LogInformation("[EventService] Updated event ID {Id}, invalidating caches", id);
            }

            return updated?.ToEventResponseDto();
        }

        /// <summary>
        /// Invalidates all event-related caches.
        /// </summary>
        private void InvalidateEventCache()
        {
            _memoryCache.Remove(CACHE_KEY_UPCOMING_EVENTS);
            _memoryCache.Remove(CACHE_KEY_ALL_EVENTS);
            _logger.LogDebug("[EventService] Invalidated all event caches");
        }

        /// <summary>
        /// Invalidates specific event cache by ID.
        /// </summary>
        private void InvalidateEventCache(int id)
        {
            _memoryCache.Remove(CACHE_KEY_UPCOMING_EVENTS);
            _memoryCache.Remove(CACHE_KEY_ALL_EVENTS);
            _memoryCache.Remove(string.Format(CACHE_KEY_EVENT_ID, id));
            _logger.LogDebug("[EventService] Invalidated event cache for ID {Id}", id);
        }
    }
}