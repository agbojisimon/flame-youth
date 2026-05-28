using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Ministry;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class MinistryService : IMinistryService
    {
        private readonly IMinistryRepository _ministryRepo;
        private readonly HybridCache _cache;
        private readonly ILogger<MinistryService> _logger;

        public MinistryService(IMinistryRepository ministryRepo, HybridCache cache, ILogger<MinistryService> logger)
        {
            _ministryRepo = ministryRepo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResult<MinistryResponseDto>> GetAllAsync(MinistryQueryObject query)
        {
            return await _cache.GetOrCreateAsync(
                CacheKeys.MinistriesAll,
                async cancel =>
                {
                    var ministries = await _ministryRepo.GetAllAsync(query);
                    var total = await _ministryRepo.GetCountAsync(query);

                    return new PagedResult<MinistryResponseDto>
                    {
                        Items = ministries.ToDtoList(),
                        TotalCount = total,
                        PageNumber = query.PageNumber,
                        PageSize = query.PageSize,
                    };
                },
                tags: [CacheKeys.TagMinistries],
                cancellationToken: CancellationToken.None);
        }

        public async Task<MinistryResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.MinistryId, id);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var ministry = await _ministryRepo.GetByIdAsync(id);
                    return ministry?.ToMinistryResponseDto();
                },
                tags: [CacheKeys.TagMinistries],
                cancellationToken: CancellationToken.None);
        }

        public async Task<MinistryResponseDto?> GetBySlugAsync(string slug)
        {
            var cacheKey = string.Format(CacheKeys.MinistrySlug, slug);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var ministry = await _ministryRepo.GetBySlugAsync(slug);
                    return ministry?.ToMinistryResponseDto();
                },
                tags: [CacheKeys.TagMinistries],
                cancellationToken: CancellationToken.None);
        }

        public async Task<MinistryResponseDto> CreateAsync(CreateMinistryDto dto)
        {
            var model = dto.ToModel();
            var created = await _ministryRepo.CreateAsync(model);

            await _cache.RemoveByTagAsync(CacheKeys.TagMinistries, CancellationToken.None);
            _logger.LogInformation("[MinistryService] Created ministry ID {Id}, invalidated cache tag {Tag}", created.Id, CacheKeys.TagMinistries);

            return created.ToMinistryResponseDto();
        }

        public async Task<MinistryResponseDto?> UpdateAsync(int id, UpdateMinistryDto dto)
        {
            var updated = await _ministryRepo.UpdateAsync(id, dto);

            if (updated != null)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagMinistries, CancellationToken.None);
                _logger.LogInformation("[MinistryService] Updated ministry ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagMinistries);
            }

            return updated?.ToMinistryResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _ministryRepo.DeleteAsync(id);

            if (result)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagMinistries, CancellationToken.None);
                _logger.LogInformation("[MinistryService] Deleted ministry ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagMinistries);
            }

            return result;
        }
    }
}
