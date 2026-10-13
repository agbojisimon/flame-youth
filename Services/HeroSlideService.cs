using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class HeroSlideService : IHeroSlideService
    {
        private readonly IHeroSlideRepository _repo;
        private readonly HybridCache _cache;
        private readonly ILogger<HeroSlideService> _logger;

        public HeroSlideService(
            IHeroSlideRepository repo,
            HybridCache cache,
            ILogger<HeroSlideService> logger)
        {
            _repo = repo;
            _cache = cache;
            _logger = logger;
        }

        public async Task<List<HeroSlideDto>> GetActiveAsync()
        {
            async ValueTask<List<HeroSlideDto>> Factory(CancellationToken ct)
            {
                var slides = await _repo.GetActiveAsync();
                return slides.ToHeroSlideDtoList();
            }

            try
            {
                return await _cache.GetOrCreateAsync(
                    CacheKeys.HeroSlidesActive,
                    Factory,
                    tags: [CacheKeys.TagHeroSlides],
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Redis unavailable, falling back to DB for cache key {CacheKey}",
                    CacheKeys.HeroSlidesActive);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<List<HeroSlideDto>> GetAllAsync()
        {
            var slides = await _repo.GetAllAsync();
            return slides.ToHeroSlideDtoList();
        }

        public async Task<HeroSlideDto?> GetByIdAsync(int id)
        {
            var slide = await _repo.GetByIdAsync(id);
            return slide?.ToHeroSlideDto();
        }

        public async Task<HeroSlideDto> CreateAsync(CreateHeroSlideDto dto)
        {
            var created = await _repo.CreateAsync(dto.ToModel());
            await InvalidateCacheAsync();
            return created.ToHeroSlideDto();
        }

        public async Task<HeroSlideDto?> UpdateAsync(int id, UpdateHeroSlideDto dto)
        {
            var updated = await _repo.UpdateAsync(id, dto);
            if (updated is null) return null;

            await InvalidateCacheAsync();
            return updated.ToHeroSlideDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleted = await _repo.DeleteAsync(id);
            if (deleted)
                await InvalidateCacheAsync();

            return deleted;
        }

        private async Task InvalidateCacheAsync()
        {
            try
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagHeroSlides, CancellationToken.None);
                _logger.LogInformation(
                    "[HeroSlideService] Invalidated cache tag {Tag}",
                    CacheKeys.TagHeroSlides);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "[HeroSlideService] Redis unavailable, cache invalidation skipped for {Tag}",
                    CacheKeys.TagHeroSlides);
            }
        }
    }
}