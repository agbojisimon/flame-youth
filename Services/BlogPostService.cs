using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Exceptions;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class BlogPostService : IBlogPostService
    {
        private const int MaxFeaturedPosts = 2;
        private const string MaxFeaturedMessage =
            "A maximum of 2 blog posts can be featured in the hero. Unfeature one first.";
        private const string CoverRequiredMessage =
            "Only published blog posts with a cover image can be featured in the hero.";

        private readonly IBlogPostRepository _blogRepository;
        private readonly HybridCache _cache;
        private readonly ILogger<BlogPostService> _logger;

        public BlogPostService(IBlogPostRepository blogRepository, HybridCache cache, ILogger<BlogPostService> logger)
        {
            _blogRepository = blogRepository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<BlogPostResponseDto> CreateAsync(CreateBlogPostDto dto, string authorId)
        {
            // A draft can never be featured; a covered, published post is required.
            if (dto.IsFeaturedOnHero && !dto.IsPublished)
                dto.IsFeaturedOnHero = false;

            if (dto.IsFeaturedOnHero && string.IsNullOrWhiteSpace(dto.CoverImageUrl))
                throw new BlogValidationException(CoverRequiredMessage);

            var blogPost = dto.ToModel(authorId);

            var created = await _blogRepository.RunInSerializableTransactionAsync(async () =>
            {
                if (dto.IsFeaturedOnHero)
                {
                    var featuredCount = await _blogRepository.CountFeaturedAsync(null);
                    if (featuredCount >= MaxFeaturedPosts)
                        throw new BlogValidationException(MaxFeaturedMessage);

                    blogPost.IsFeaturedOnHero = true;
                    blogPost.FeaturedOnHeroAt = DateTime.UtcNow;
                }
                else
                {
                    blogPost.IsFeaturedOnHero = false;
                    blogPost.FeaturedOnHeroAt = null;
                }

                return await _blogRepository.CreateAsync(blogPost);
            });

            try
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
                _logger.LogInformation("[BlogPostService] Created blog post ID {Id}, invalidated cache tag {Tag}", created.Id, CacheKeys.TagBlog);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[BlogPostService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagBlog);
            }

            return created.ToResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _blogRepository.DeleteAsync(id);

            if (result)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
                    _logger.LogInformation("[BlogPostService] Deleted blog post ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBlog);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[BlogPostService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagBlog);
                }
            }

            return result;
        }

        public async Task<PagedResult<BlogPostResponseDto>> GetAllAsync(BlogQueryObject query)
        {
            var cacheKey = string.Format(
                CacheKeys.BlogPublished,
                query.PageNumber,
                query.PageSize,
                query.Department ?? string.Empty,
                query.SearchTerm ?? string.Empty,
                query.IsPublished?.ToString() ?? string.Empty,
                query.FeaturedOnHero?.ToString() ?? string.Empty);

            async ValueTask<PagedResult<BlogPostResponseDto>> Factory(CancellationToken ct)
            {
                var posts = await _blogRepository.GetAllAsync(query);
                var totalCount = await _blogRepository.GetCountAsync(query);

                return new PagedResult<BlogPostResponseDto>
                {
                    Items = posts.ToDtoList(),
                    TotalCount = totalCount,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize
                };
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBlog], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<BlogPostResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.BlogId, id);

            async ValueTask<BlogPostResponseDto?> Factory(CancellationToken ct)
            {
                var post = await _blogRepository.GetByIdAsync(id);
                return post?.ToResponseDto();
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBlog], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<BlogPostResponseDto?> GetBySlugAsync(string slug)
        {
            var cacheKey = string.Format(CacheKeys.BlogSlug, slug);

            async ValueTask<BlogPostResponseDto?> Factory(CancellationToken ct)
            {
                var post = await _blogRepository.GetBySlugAsync(slug);
                return post?.ToResponseDto();
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBlog], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<BlogPostResponseDto?> UpdateAsync(int id, UpdateBlogPostDto dto)
        {
            // Unpublishing always removes the post from the hero.
            if (dto.IsFeaturedOnHero && !dto.IsPublished)
                dto.IsFeaturedOnHero = false;

            if (dto.IsFeaturedOnHero && string.IsNullOrWhiteSpace(dto.CoverImageUrl))
                throw new BlogValidationException(CoverRequiredMessage);

            var updated = await _blogRepository.RunInSerializableTransactionAsync(async () =>
            {
                if (dto.IsFeaturedOnHero)
                {
                    var featuredCount = await _blogRepository.CountFeaturedAsync(id);
                    if (featuredCount >= MaxFeaturedPosts)
                        throw new BlogValidationException(MaxFeaturedMessage);
                }

                return await _blogRepository.UpdateAsync(id, dto);
            });

            if (updated is not null)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
                    _logger.LogInformation("[BlogPostService] Updated blog post ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBlog);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[BlogPostService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagBlog);
                }
            }

            return updated?.ToResponseDto();
        }

        public async Task<bool> TogglePublishAsync(int id)
        {
            var result = await _blogRepository.TogglePublishAsync(id);

            if (result)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
                    _logger.LogInformation("[BlogPostService] Toggled publish blog post ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBlog);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[BlogPostService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagBlog);
                }
            }

            return result;
        }

        public async Task<List<BlogDepartmentDto>> GetDistinctDepartmentsAsync()
        {
            var cacheKey = CacheKeys.BlogDepartments;

            async ValueTask<List<BlogDepartmentDto>> Factory(CancellationToken ct)
            {
                return await _blogRepository.GetDistinctDepartmentsAsync(publishedOnly: true);
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBlog], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<List<BlogPostResponseDto>> GetRelatedAsync(string slug, int take)
        {
            if (take <= 0) take = 3;
            if (take > 10) take = 10;

            var cacheKey = string.Format(CacheKeys.BlogRelated, slug, take);

            async ValueTask<List<BlogPostResponseDto>> Factory(CancellationToken ct)
            {
                var current = await _blogRepository.GetBySlugAsync(slug);

                if (current is null || !current.IsPublished || current.IsDeleted)
                    return new List<BlogPostResponseDto>();

                var related = await _blogRepository.GetRelatedAsync(current.Id, current.Department, take);
                return related.ToDtoList();
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBlog], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }
    }
}
