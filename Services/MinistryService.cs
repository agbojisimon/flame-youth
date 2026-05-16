using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Ministry;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Memory;

namespace GlobalFlameMinistry.API.Services
{
    public class MinistryService : IMinistryService
    {
        private readonly IMinistryRepository _ministryRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<MinistryService> _logger;

        // Cache key constants
        private const string CACHE_KEY_ALL_MINISTRIES = "ministries_all";
        private const string CACHE_KEY_MINISTRY_SLUG = "ministry_slug_{0}";
        private const string CACHE_KEY_MINISTRY_ID = "ministry_id_{0}";

        // Cache expiration times
        private readonly TimeSpan _listCacheExpiration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _itemCacheExpiration = TimeSpan.FromMinutes(10);

        public MinistryService(IMinistryRepository ministryRepo, IMemoryCache memoryCache, ILogger<MinistryService> logger)
        {
            _ministryRepo = ministryRepo;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Gets all ministries with caching. Cache expires after 5 minutes.
        /// </summary>
        public async Task<PagedResult<MinistryResponseDto>> GetAllAsync(MinistryQueryObject query)
        {
            // Use cache key without pagination to maintain consistent caching strategy
            string cacheKey = CACHE_KEY_ALL_MINISTRIES;

            // Attempt to retrieve from cache
            if (_memoryCache.TryGetValue(cacheKey, out PagedResult<MinistryResponseDto>? cachedResult))
            {
                _logger.LogInformation("[MinistryService] Cache hit for all ministries");
                return cachedResult!;
            }

            _logger.LogInformation("[MinistryService] Cache miss for all ministries - querying database");

            var ministries = await _ministryRepo.GetAllAsync(query);
            var total = await _ministryRepo.GetCountAsync(query);

            var result = new PagedResult<MinistryResponseDto>
            {
                Items = ministries.ToDtoList(),
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            };

            // Store in cache with 5-minute expiration
            _memoryCache.Set(cacheKey, result, _listCacheExpiration);

            return result;
        }

        /// <summary>
        /// Gets a ministry by ID with caching. Cache expires after 10 minutes.
        /// </summary>
        public async Task<MinistryResponseDto?> GetByIdAsync(int id)
        {
            string cacheKey = string.Format(CACHE_KEY_MINISTRY_ID, id);

            if (_memoryCache.TryGetValue(cacheKey, out MinistryResponseDto? cachedMinistry))
            {
                _logger.LogInformation("[MinistryService] Cache hit for ministry ID {Id}", id);
                return cachedMinistry;
            }

            _logger.LogInformation("[MinistryService] Cache miss for ministry ID {Id}", id);

            var ministry = await _ministryRepo.GetByIdAsync(id);
            var result = ministry?.ToMinistryResponseDto();

            if (result != null)
            {
                _memoryCache.Set(cacheKey, result, _itemCacheExpiration);
            }

            return result;
        }

        /// <summary>
        /// Gets a ministry by slug with caching. Cache expires after 10 minutes.
        /// </summary>
        public async Task<MinistryResponseDto?> GetBySlugAsync(string slug)
        {
            string cacheKey = string.Format(CACHE_KEY_MINISTRY_SLUG, slug);

            if (_memoryCache.TryGetValue(cacheKey, out MinistryResponseDto? cachedMinistry))
            {
                _logger.LogInformation("[MinistryService] Cache hit for ministry slug {Slug}", slug);
                return cachedMinistry;
            }

            _logger.LogInformation("[MinistryService] Cache miss for ministry slug {Slug}", slug);

            var ministry = await _ministryRepo.GetBySlugAsync(slug);
            var result = ministry?.ToMinistryResponseDto();

            if (result != null)
            {
                _memoryCache.Set(cacheKey, result, _itemCacheExpiration);
            }

            return result;
        }

        /// <summary>
        /// Creates a new ministry (admin endpoint). Invalidates all ministry caches.
        /// </summary>
        public async Task<MinistryResponseDto> CreateAsync(CreateMinistryDto dto)
        {
            var model = dto.ToModel();
            var created = await _ministryRepo.CreateAsync(model);

            // Invalidate all ministry caches
            InvalidateMinistryCache();

            _logger.LogInformation("[MinistryService] Created new ministry with ID {Id}, invalidating caches", created.Id);

            return created.ToMinistryResponseDto();
        }

        /// <summary>
        /// Updates an existing ministry (admin endpoint). Invalidates related caches.
        /// </summary>
        public async Task<MinistryResponseDto?> UpdateAsync(int id, UpdateMinistryDto dto)
        {
            var updated = await _ministryRepo.UpdateAsync(id, dto);

            if (updated != null)
            {
                // Invalidate specific ministry caches
                InvalidateMinistryCache(id, updated.Slug);
                _logger.LogInformation("[MinistryService] Updated ministry ID {Id}, invalidating caches", id);
            }

            return updated?.ToMinistryResponseDto();
        }

        /// <summary>
        /// Deletes a ministry (admin endpoint). Invalidates all ministry caches.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _ministryRepo.DeleteAsync(id);

            if (result)
            {
                // Invalidate all ministry caches
                InvalidateMinistryCache();
                _logger.LogInformation("[MinistryService] Deleted ministry ID {Id}, invalidating caches", id);
            }

            return result;
        }

        /// <summary>
        /// Invalidates all ministry-related caches.
        /// </summary>
        private void InvalidateMinistryCache()
        {
            _memoryCache.Remove(CACHE_KEY_ALL_MINISTRIES);
            _logger.LogDebug("[MinistryService] Invalidated all ministries cache");
        }

        /// <summary>
        /// Invalidates specific ministry caches by ID and slug.
        /// </summary>
        private void InvalidateMinistryCache(int id, string? slug)
        {
            _memoryCache.Remove(CACHE_KEY_ALL_MINISTRIES);
            _memoryCache.Remove(string.Format(CACHE_KEY_MINISTRY_ID, id));

            if (!string.IsNullOrWhiteSpace(slug))
            {
                _memoryCache.Remove(string.Format(CACHE_KEY_MINISTRY_SLUG, slug));
            }

            _logger.LogDebug("[MinistryService] Invalidated ministry caches for ID {Id} and slug {Slug}", id, slug);
        }
    }
}