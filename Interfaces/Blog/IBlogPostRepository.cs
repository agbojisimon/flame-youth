using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<BlogPost> CreateAsync(BlogPost post);
        Task<BlogPost?> GetByIdAsync(int id);
        Task<BlogPost?> GetBySlugAsync(string slug);
        Task<List<BlogPost>> GetAllAsync(BlogQueryObject query);
        Task<int> GetCountAsync(BlogQueryObject query);
        Task<BlogPost?> UpdateAsync(int id, UpdateBlogPostDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> TogglePublishAsync(int id);
    }
}
