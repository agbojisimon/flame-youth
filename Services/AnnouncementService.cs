using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Memory;

namespace GlobalFlameMinistry.API.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announceRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AnnouncementService> _logger;

        // Cache key constants
        private const string CACHE_KEY_PUBLISHED_ANNOUNCEMENTS = "announcements_published";
        private const string CACHE_KEY_ALL_ANNOUNCEMENTS = "announcements_all";
        private const string CACHE_KEY_ANNOUNCEMENT_ID = "announcement_id_{0}";

        // Cache expiration times
        private readonly TimeSpan _listCacheExpiration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _itemCacheExpiration = TimeSpan.FromMinutes(10);

        public AnnouncementService(IAnnouncementRepository announceRepo, IMemoryCache memoryCache, ILogger<AnnouncementService> logger)
        {
            _announceRepo = announceRepo;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new announcement (admin endpoint). Invalidates announcement caches.
        /// </summary>
        public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto, string createdById)
        {
            var announcement = dto.ToAnnouncementFromCreateDto(createdById);
            var created = await _announceRepo.CreateAsync(announcement);

            // Invalidate announcement caches
            InvalidateAnnouncementCache();

            _logger.LogInformation("[AnnouncementService] Created new announcement with ID {Id}, invalidating caches", created.Id);

            return created.ToAnnouncementDto();
        }

        /// <summary>
        /// Deletes an announcement (admin endpoint). Invalidates all announcement caches.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _announceRepo.DeleteAsync(id);

            if (result)
            {
                // Invalidate all announcement caches
                InvalidateAnnouncementCache();
                _logger.LogInformation("[AnnouncementService] Deleted announcement ID {Id}, invalidating caches", id);
            }

            return result;
        }

        /// <summary>
        /// Gets all announcements with caching. Cache expires after 5 minutes.
        /// Typically filters for published announcements on public endpoints.
        /// </summary>
        public async Task<PagedResult<AnnouncementDto>> GetAllAsync(AnnouncementQueryObject query)
        {
            string cacheKey = CACHE_KEY_PUBLISHED_ANNOUNCEMENTS;

            // Attempt to retrieve from cache
            if (_memoryCache.TryGetValue(cacheKey, out PagedResult<AnnouncementDto>? cachedResult))
            {
                _logger.LogInformation("[AnnouncementService] Cache hit for published announcements");
                return cachedResult!;
            }

            _logger.LogInformation("[AnnouncementService] Cache miss for published announcements - querying database");

            var announcements = await _announceRepo.GetAllAsync(query);
            var totalCount = await _announceRepo.GetCountAsync(query);

            var result = new PagedResult<AnnouncementDto>
            {
                Items = announcements.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };

            // Store in cache with 5-minute expiration
            _memoryCache.Set(cacheKey, result, _listCacheExpiration);

            return result;
        }

        /// <summary>
        /// Gets an announcement by ID with caching. Cache expires after 10 minutes.
        /// </summary>
        public async Task<AnnouncementDto?> GetByIdAsync(int id)
        {
            string cacheKey = string.Format(CACHE_KEY_ANNOUNCEMENT_ID, id);

            if (_memoryCache.TryGetValue(cacheKey, out AnnouncementDto? cachedAnnouncement))
            {
                _logger.LogInformation("[AnnouncementService] Cache hit for announcement ID {Id}", id);
                return cachedAnnouncement;
            }

            _logger.LogInformation("[AnnouncementService] Cache miss for announcement ID {Id}", id);

            var announcement = await _announceRepo.GetByIdAsync(id);

            // Return null if not found 
            if (announcement is null) return null;

            var result = announcement.ToAnnouncementDto();
            _memoryCache.Set(cacheKey, result, _itemCacheExpiration);

            return result;
        }

        /// <summary>
        /// Updates an existing announcement (admin endpoint). Invalidates related announcement caches.
        /// </summary>
        public async Task<AnnouncementDto?> UpdateAsync(int id, UpdateAnnouncementDto dto)
        {
            var updated = await _announceRepo.UpdateAsync(id, dto);

            if (updated is not null)
            {
                // Invalidate specific announcement caches
                InvalidateAnnouncementCache(id);
                _logger.LogInformation("[AnnouncementService] Updated announcement ID {Id}, invalidating caches", id);
            }

            return updated?.ToAnnouncementDto();
        }

        /// <summary>
        /// Invalidates all announcement-related caches.
        /// </summary>
        private void InvalidateAnnouncementCache()
        {
            _memoryCache.Remove(CACHE_KEY_PUBLISHED_ANNOUNCEMENTS);
            _memoryCache.Remove(CACHE_KEY_ALL_ANNOUNCEMENTS);
            _logger.LogDebug("[AnnouncementService] Invalidated all announcement caches");
        }

        /// <summary>
        /// Invalidates specific announcement cache by ID.
        /// </summary>
        private void InvalidateAnnouncementCache(int id)
        {
            _memoryCache.Remove(CACHE_KEY_PUBLISHED_ANNOUNCEMENTS);
            _memoryCache.Remove(CACHE_KEY_ALL_ANNOUNCEMENTS);
            _memoryCache.Remove(string.Format(CACHE_KEY_ANNOUNCEMENT_ID, id));
            _logger.LogDebug("[AnnouncementService] Invalidated announcement cache for ID {Id}", id);
        }
    }
}