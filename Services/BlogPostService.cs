using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class BlogPostService : IBlogPostService
    {
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
            var blogPost = dto.ToModel(authorId);
            var created = await _blogRepository.CreateAsync(blogPost);

            await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
            _logger.LogInformation("[BlogPostService] Created blog post ID {Id}, invalidated cache tag {Tag}", created.Id, CacheKeys.TagBlog);

            return created.ToResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _blogRepository.DeleteAsync(id);

            if (result)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
                _logger.LogInformation("[BlogPostService] Deleted blog post ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBlog);
            }

            return result;
        }

        public async Task<PagedResult<BlogPostResponseDto>> GetAllAsync(BlogQueryObject query)
        {
            var cacheKey = string.Format(CacheKeys.BlogPublished, query.PageNumber, query.PageSize);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
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
                },
                tags: [CacheKeys.TagBlog],
                cancellationToken: CancellationToken.None);
        }

        public async Task<BlogPostResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.BlogId, id);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var post = await _blogRepository.GetByIdAsync(id);
                    return post?.ToResponseDto();
                },
                tags: [CacheKeys.TagBlog],
                cancellationToken: CancellationToken.None);
        }

        public async Task<BlogPostResponseDto?> GetBySlugAsync(string slug)
        {
            var cacheKey = string.Format(CacheKeys.BlogSlug, slug);

            return await _cache.GetOrCreateAsync(
                cacheKey,
                async cancel =>
                {
                    var post = await _blogRepository.GetBySlugAsync(slug);
                    return post?.ToResponseDto();
                },
                tags: [CacheKeys.TagBlog],
                cancellationToken: CancellationToken.None);
        }

        public async Task<BlogPostResponseDto?> UpdateAsync(int id, UpdateBlogPostDto dto)
        {
            var updated = await _blogRepository.UpdateAsync(id, dto);

            if (updated is not null)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
                _logger.LogInformation("[BlogPostService] Updated blog post ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBlog);
            }

            return updated?.ToResponseDto();
        }

        public async Task<bool> TogglePublishAsync(int id)
        {
            var result = await _blogRepository.TogglePublishAsync(id);

            if (result)
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagBlog, CancellationToken.None);
                _logger.LogInformation("[BlogPostService] Toggled publish blog post ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBlog);
            }

            return result;
        }

        public async Task<List<string>> GetDistinctDepartmentsAsync()
        {
            return await _blogRepository.GetDistinctDepartmentsAsync(publishedOnly: true);
        }
    }
}
