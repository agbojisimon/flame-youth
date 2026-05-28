using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IAnnouncementRepository _announceRepo;
        private readonly HybridCache _cache;
        private readonly ILogger<AnnouncementService> _logger;

        public AnnouncementService(IAnnouncementRepository announceRepo, HybridCache cache, ILogger<AnnouncementService> logger)
        {
            _announceRepo = announceRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto, string createdById)
        {
            var announcement = dto.ToAnnouncementFromCreateDto(createdById);
            var created = await _announceRepo.CreateAsync(announcement);

            await _cache.RemoveByTagAsync(CacheKeys.TagAnnouncements, CancellationToken.None);
            _logger.LogInformation("[AnnouncementService] Created announcement ID {Id}, invalidated cache tag {Tag}", created.Id, CacheKeys.TagAnnouncements);

            return created.ToAnnouncementDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _announceRepo.DeleteAsync(id);

            if (result)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagAnnouncements, CancellationToken.None);
                _logger.LogInformation("[AnnouncementService] Deleted announcement ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagAnnouncements);
            }

            return result;
        }

        public async Task<PagedResult<AnnouncementDto>> GetAllAsync(AnnouncementQueryObject query)
        {
            return await _cache.GetOrCreateAsync(
                CacheKeys.AnnouncementsPublished,
                async cancel =>
                {
                    var announcements = await _announceRepo.GetAllAsync(query);
                    var totalCount = await _announceRepo.GetCountAsync(query);

                    return new PagedResult<AnnouncementDto>
                    {
                        Items = announcements.ToDtoList(),
                        TotalCount = totalCount,
                        PageNumber = query.PageNumber,
                        PageSize = query.PageSize
                    };
                },
                tags: [CacheKeys.TagAnnouncements],
                cancellationToken: CancellationToken.None);
        }

        public async Task<AnnouncementDto?> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.AnnouncementId, id);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var announcement = await _announceRepo.GetByIdAsync(id);
                    return announcement?.ToAnnouncementDto();
                },
                tags: [CacheKeys.TagAnnouncements],
                cancellationToken: CancellationToken.None);
        }

        public async Task<AnnouncementDto?> GetBySlugAsync(string slug)
        {
            var cacheKey = string.Format(CacheKeys.AnnouncementSlug, slug);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var announcement = await _announceRepo.GetBySlugAsync(slug);
                    return announcement?.ToAnnouncementDto();
                },
                tags: [CacheKeys.TagAnnouncements],
                cancellationToken: CancellationToken.None);
        }

        public async Task<AnnouncementDto?> UpdateAsync(int id, UpdateAnnouncementDto dto)
        {
            var updated = await _announceRepo.UpdateAsync(id, dto);

            if (updated is not null)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagAnnouncements, CancellationToken.None);
                _logger.LogInformation("[AnnouncementService] Updated announcement ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagAnnouncements);
            }

            return updated?.ToAnnouncementDto();
        }
    }
}
