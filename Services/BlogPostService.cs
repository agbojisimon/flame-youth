using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly IBlogPostRepository _blogRepository;

        public BlogPostService(IBlogPostRepository blogRepository)
        {
            _blogRepository = blogRepository;
        }

        public async Task<BlogPostResponseDto> CreateAsync(CreateBlogPostDto dto, string authorId)
        {
            var blogPost = dto.ToModel(authorId);
            var created = await _blogRepository.CreateAsync(blogPost);
            return created.ToResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _blogRepository.DeleteAsync(id);
        }

        public async Task<PagedResult<BlogPostResponseDto>> GetAllAsync(BlogQueryObject query)
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

        public async Task<BlogPostResponseDto?> GetByIdAsync(int id)
        {
            var post = await _blogRepository.GetByIdAsync(id);
            return post?.ToResponseDto();
        }

        public async Task<BlogPostResponseDto?> GetBySlugAsync(string slug)
        {
            var post = await _blogRepository.GetBySlugAsync(slug);
            return post?.ToResponseDto();
        }

        public async Task<BlogPostResponseDto?> UpdateAsync(int id, UpdateBlogPostDto dto)
        {
            var updated = await _blogRepository.UpdateAsync(id, dto);
            return updated?.ToResponseDto();
        }

        public async Task<bool> TogglePublishAsync(int id)
        {
            return await _blogRepository.TogglePublishAsync(id);
        }

        public async Task<List<string>> GetDistinctDepartmentsAsync()
        {
            return await _blogRepository.GetDistinctDepartmentsAsync(
                publishedOnly: true);
        }
    }
}
