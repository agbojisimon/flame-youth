using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Exceptions;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class HeroSlideService : IHeroSlideService
    {
        private const int MaxFeaturedClips = 3;
        private const string MaxFeaturedMessage =
            "A maximum of 3 clips can be featured on the home page. Unfeature one first.";
        private const string VideoOnlyMessage =
            "Only video clips can be featured on the home page.";

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

        public async Task<List<HeroSlideDto>> GetClipsAsync()
        {
            async ValueTask<List<HeroSlideDto>> Factory(CancellationToken ct)
            {
                var clips = await _repo.GetClipsAsync();
                return clips.ToHeroSlideDtoList();
            }

            try
            {
                return await _cache.GetOrCreateAsync(
                    CacheKeys.HeroClips,
                    Factory,
                    tags: [CacheKeys.TagHeroSlides],
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Redis unavailable, falling back to DB for cache key {CacheKey}",
                    CacheKeys.HeroClips);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<List<HeroSlideDto>> GetFeaturedClipsAsync()
        {
            async ValueTask<List<HeroSlideDto>> Factory(CancellationToken ct)
            {
                var clips = await _repo.GetFeaturedClipsAsync();
                return clips.ToHeroSlideDtoList();
            }

            try
            {
                return await _cache.GetOrCreateAsync(
                    CacheKeys.HeroClipsFeatured,
                    Factory,
                    tags: [CacheKeys.TagHeroSlides],
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Redis unavailable, falling back to DB for cache key {CacheKey}",
                    CacheKeys.HeroClipsFeatured);
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
            return slide?.ToHeroSlideDetailDto();
        }

        public async Task<HeroSlideDto?> GetClipBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return null;

            var cacheKey = string.Format(CacheKeys.HeroClipSlug, slug);

            async ValueTask<HeroSlideDto?> Factory(CancellationToken ct)
            {
                var slide = await _repo.GetBySlugAsync(slug);
                return slide?.ToHeroSlideDetailDto();
            }

            try
            {
                return await _cache.GetOrCreateAsync(
                    cacheKey,
                    Factory,
                    tags: [CacheKeys.TagHeroSlides],
                    cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Redis unavailable, falling back to DB for cache key {CacheKey}",
                    cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<HeroSlideDto> CreateAsync(CreateHeroSlideDto dto)
        {
            if (dto.IsFeatured && !IsVideo(dto.MediaType))
                throw new HeroSlideValidationException(VideoOnlyMessage);

            var created = await _repo.RunInSerializableTransactionAsync(async () =>
            {
                var model = dto.ToModel();

                if (dto.IsFeatured && IsVideo(dto.MediaType) && dto.IsActive)
                {
                    var featuredCount = await _repo.CountFeaturedAsync(null);
                    if (featuredCount >= MaxFeaturedClips)
                        throw new HeroSlideValidationException(MaxFeaturedMessage);

                    model.IsFeatured = true;
                    model.FeaturedOrder = await _repo.GetNextFeaturedOrderAsync();
                }
                else
                {
                    model.IsFeatured = false;
                    model.FeaturedOrder = 0;
                }

                return await _repo.CreateAsync(model);
            });

            await InvalidateCacheAsync();
            return created.ToHeroSlideDto();
        }

        public async Task<HeroSlideDto?> UpdateAsync(int id, UpdateHeroSlideDto dto)
        {
            if (dto.IsFeatured && !IsVideo(dto.MediaType))
                throw new HeroSlideValidationException(VideoOnlyMessage);

            var updated = await _repo.RunInSerializableTransactionAsync(async () =>
            {
                var existing = await _repo.GetByIdAsync(id);
                if (existing is null) return null;

                var wasFeatured =
                    existing.IsFeatured && existing.MediaType == "Video" && existing.IsActive;
                var willFeature = dto.IsFeatured && IsVideo(dto.MediaType) && dto.IsActive;

                if (willFeature)
                {
                    dto.IsFeatured = true;

                    if (wasFeatured)
                    {
                        dto.FeaturedOrder = existing.FeaturedOrder;
                    }
                    else
                    {
                        var featuredCount = await _repo.CountFeaturedAsync(id);
                        if (featuredCount >= MaxFeaturedClips)
                            throw new HeroSlideValidationException(MaxFeaturedMessage);

                        dto.FeaturedOrder = await _repo.GetNextFeaturedOrderAsync();
                    }
                }
                else
                {
                    dto.IsFeatured = false;
                    dto.FeaturedOrder = 0;
                }

                return await _repo.UpdateAsync(id, dto);
            });

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

        private static bool IsVideo(string? mediaType) =>
            string.Equals(mediaType, "Video", StringComparison.OrdinalIgnoreCase);

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
