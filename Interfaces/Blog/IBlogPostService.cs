using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IBlogPostService
    {
        Task<BlogPostResponseDto> CreateAsync(CreateBlogPostDto dto, string authorId);
        Task<PagedResult<BlogPostResponseDto>> GetAllAsync(BlogQueryObject query);
        Task<BlogPostResponseDto?> GetByIdAsync(int id);
        Task<BlogPostResponseDto?> GetBySlugAsync(string slug);
        Task<BlogPostResponseDto?> UpdateAsync(int id, UpdateBlogPostDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> TogglePublishAsync(int id);
    }
}
