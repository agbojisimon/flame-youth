using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class SermonService : ISermonService
    {
        private readonly ISermonRepository _repository;
        private readonly HybridCache _cache;
        private readonly ILogger<SermonService> _logger;

        public SermonService(ISermonRepository repository, HybridCache cache, ILogger<SermonService> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResult<SermonResponseDto>> GetPublishedAsync(SermonQueryObject query)
        {
            query.IsPublished = true;

            var cacheKey = string.Format(CacheKeys.SermonPublished,
                query.PageNumber, query.PageSize, query.IsFeatured?.ToString() ?? "null");

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var sermons = await _repository.GetAllAsync(query);
                    var total = await _repository.GetCountAsync(query);

                    return new PagedResult<SermonResponseDto>
                    {
                        Items = sermons.ToDtoList(),
                        TotalCount = total,
                        PageNumber = query.PageNumber,
                        PageSize = query.PageSize
                    };
                },
                tags: [CacheKeys.TagSermons],
                cancellationToken: CancellationToken.None);
        }

        public async Task<SermonResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.SermonId, id);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var sermon = await _repository.GetByIdAsync(id);
                    return sermon?.ToDto();
                },
                tags: [CacheKeys.TagSermons],
                cancellationToken: CancellationToken.None);
        }

        public async Task<SermonResponseDto?> GetBySlugAsync(string slug)
        {
            var cacheKey = string.Format(CacheKeys.SermonSlug, slug);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var sermon = await _repository.GetBySlugAsync(slug);
                    return sermon?.ToDto();
                },
                tags: [CacheKeys.TagSermons],
                cancellationToken: CancellationToken.None);
        }

        public async Task<PagedResult<SermonResponseDto>> GetAllAsync(SermonQueryObject query)
        {
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

        public async Task<SermonResponseDto> CreateAsync(CreateSermonDto dto)
        {
            var sermon = dto.ToModel();
            var created = await _repository.CreateAsync(sermon);

            await _cache.RemoveByTagAsync(CacheKeys.TagSermons, CancellationToken.None);
            _logger.LogInformation("[SermonService] Created sermon ID {Id}, invalidated cache tag {Tag}", created.Id, CacheKeys.TagSermons);

            return created.ToDto();
        }

        public async Task<SermonResponseDto?> UpdateAsync(int id, UpdateSermonDto dto)
        {
            var updated = await _repository.UpdateAsync(id, dto);

            if (updated is not null)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagSermons, CancellationToken.None);
                _logger.LogInformation("[SermonService] Updated sermon ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagSermons);
            }

            return updated?.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagSermons, CancellationToken.None);
                _logger.LogInformation("[SermonService] Deleted sermon ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagSermons);
            }

            return result;
        }

        public async Task<SermonResponseDto?> ToggleFeaturedAsync(int id)
        {
            var sermon = await _repository.GetByIdAsync(id);
            if (sermon is null) return null;
            sermon.IsFeatured = !sermon.IsFeatured;
            await _repository.SaveChangesAsync();

            await _cache.RemoveByTagAsync(CacheKeys.TagSermons, CancellationToken.None);
            _logger.LogInformation("[SermonService] Toggled featured sermon ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagSermons);

            return sermon.ToDto();
        }
    }
}
