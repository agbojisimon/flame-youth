using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Memory;

namespace GlobalFlameMinistry.API.Services
{
    public class SermonService : ISermonService
    {
        private readonly ISermonRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<SermonService> _logger;

        // Cache key constants
        private const string CACHE_KEY_PUBLISHED_SERMONS = "sermons_published";
        private const string CACHE_KEY_ALL_SERMONS = "sermons_all";
        private const string CACHE_KEY_SERMON_ID = "sermon_id_{0}";

        // Cache expiration times
        private readonly TimeSpan _listCacheExpiration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _itemCacheExpiration = TimeSpan.FromMinutes(10);

        public SermonService(ISermonRepository repository, IMemoryCache memoryCache, ILogger<SermonService> logger)
        {
            _repository = repository;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Gets all published sermons with caching. Cache expires after 5 minutes.
        /// This is a public-facing endpoint, so caching is applied.
        /// </summary>
        public async Task<PagedResult<SermonResponseDto>> GetPublishedAsync(SermonQueryObject query)
        {
            // Force published only for public
            query.IsPublished = true;

            string cacheKey = $"{CACHE_KEY_PUBLISHED_SERMONS}_p{query.PageNumber}_s{query.PageSize}_{query.SortBy}_{query.IsDescending}_featured{query.IsFeatured}";

            // Attempt to retrieve from cache
            if (_memoryCache.TryGetValue(cacheKey, out PagedResult<SermonResponseDto>? cachedResult))
            {
                _logger.LogInformation("[SermonService] Cache hit for published sermons");
                return cachedResult!;
            }

            _logger.LogInformation("[SermonService] Cache miss for published sermons - querying database");

            var sermons = await _repository.GetAllAsync(query);
            var total = await _repository.GetCountAsync(query);

            var result = new PagedResult<SermonResponseDto>
            {
                Items = sermons.ToDtoList(),
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            // Store in cache with 5-minute expiration
            _memoryCache.Set(cacheKey, result, _listCacheExpiration);

            return result;
        }

        /// <summary>
        /// Gets a sermon by ID with caching. Cache expires after 10 minutes.
        /// </summary>
        public async Task<SermonResponseDto?> GetByIdAsync(int id)
        {
            string cacheKey = string.Format(CACHE_KEY_SERMON_ID, id);

            if (_memoryCache.TryGetValue(cacheKey, out SermonResponseDto? cachedSermon))
            {
                _logger.LogInformation("[SermonService] Cache hit for sermon ID {Id}", id);
                return cachedSermon;
            }

            _logger.LogInformation("[SermonService] Cache miss for sermon ID {Id}", id);

            var sermon = await _repository.GetByIdAsync(id);

            if (sermon is null)
                return null;

            var result = sermon.ToDto();
            _memoryCache.Set(cacheKey, result, _itemCacheExpiration);

            return result;
        }

        /// <summary>
        /// Gets a sermon by slug with caching. Cache expires after 10 minutes.
        /// </summary>
        public async Task<SermonResponseDto?> GetBySlugAsync(string slug)
        {
            string cacheKey = $"sermon_slug_{slug}";

            if (_memoryCache.TryGetValue(cacheKey, out SermonResponseDto? cachedSermon))
            {
                _logger.LogInformation("[SermonService] Cache hit for sermon slug {Slug}", slug);
                return cachedSermon;
            }

            _logger.LogInformation("[SermonService] Cache miss for sermon slug {Slug}", slug);

            var sermon = await _repository.GetBySlugAsync(slug);

            if (sermon is null) return null;

            var result = sermon.ToDto();
            _memoryCache.Set(cacheKey, result, _itemCacheExpiration);

            return result;
        }

        /// <summary>
        /// Gets all sermons (including unpublished). Admin endpoint - does not cache.
        /// </summary>
        public async Task<PagedResult<SermonResponseDto>> GetAllAsync(SermonQueryObject query)
        {
            // No caching for admin endpoint
            var sermons = await _repository.GetAllAsync(query);
            var total = await _repository.GetCountAsync(query);

            return new PagedResult<SermonResponseDto>
            {
                Items = sermons.ToDtoList(),
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        /// <summary>
        /// Creates a new sermon (admin endpoint). Invalidates sermon caches.
        /// </summary>
        public async Task<SermonResponseDto> CreateAsync(CreateSermonDto dto)
        {
            var sermon = dto.ToModel();
            var created = await _repository.CreateAsync(sermon);

            // Invalidate sermon caches
            InvalidateSermonCache();

            _logger.LogInformation("[SermonService] Created new sermon with ID {Id}, invalidating caches", created.Id);

            return created.ToDto();
        }

        /// <summary>
        /// Updates an existing sermon (admin endpoint). Invalidates related sermon caches.
        /// </summary>
        public async Task<SermonResponseDto?> UpdateAsync(int id, UpdateSermonDto dto)
        {
            var updated = await _repository.UpdateAsync(id, dto);

            if (updated is not null)
            {
                // Invalidate specific sermon caches
                InvalidateSermonCache(id);
                _logger.LogInformation("[SermonService] Updated sermon ID {Id}, invalidating caches", id);
            }

            return updated?.ToDto();
        }

        /// <summary>
        /// Deletes a sermon (admin endpoint). Invalidates all sermon caches.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                // Invalidate all sermon caches
                InvalidateSermonCache();
                _logger.LogInformation("[SermonService] Deleted sermon ID {Id}, invalidating caches", id);
            }

            return result;
        }

        public async Task<SermonResponseDto?> ToggleFeaturedAsync(int id)
        {
            var sermon = await _repository.GetByIdAsync(id);
            if (sermon is null) return null;
            sermon.IsFeatured = !sermon.IsFeatured;
            await _repository.SaveChangesAsync();
            InvalidateSermonCache(id);
            return sermon.ToDto();
        }

        /// <summary>
        /// Invalidates all sermon-related caches.
        /// </summary>
        private void InvalidateSermonCache()
        {
            _memoryCache.Remove(CACHE_KEY_PUBLISHED_SERMONS);
            _memoryCache.Remove(CACHE_KEY_ALL_SERMONS);
            _logger.LogDebug("[SermonService] Invalidated all sermon caches");
        }

        /// <summary>
        /// Invalidates specific sermon cache by ID.
        /// </summary>
        private void InvalidateSermonCache(int id)
        {
            _memoryCache.Remove(CACHE_KEY_PUBLISHED_SERMONS);
            _memoryCache.Remove(CACHE_KEY_ALL_SERMONS);
            _memoryCache.Remove(string.Format(CACHE_KEY_SERMON_ID, id));
            _logger.LogDebug("[SermonService] Invalidated sermon cache for ID {Id}", id);
        }
    }
}