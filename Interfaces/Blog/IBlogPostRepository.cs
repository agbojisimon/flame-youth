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
        Task<List<BlogDepartmentDto>> GetDistinctDepartmentsAsync(bool publishedOnly = true);
        Task<List<BlogPost>> GetRelatedAsync(int currentId, string department, int take);
        Task<int> CountFeaturedAsync(int? excludeId);
        Task<T> RunInSerializableTransactionAsync<T>(Func<Task<T>> operation);
        Task<BlogPost?> UpdateAsync(int id, UpdateBlogPostDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> TogglePublishAsync(int id);
    }
}
