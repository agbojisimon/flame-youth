using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IHeroSlideRepository
    {
        Task<List<HeroSlide>> GetActiveAsync();
        Task<List<HeroSlide>> GetClipsAsync();
        Task<List<HeroSlide>> GetFeaturedClipsAsync();
        Task<List<HeroSlide>> GetAllAsync();
        Task<HeroSlide?> GetByIdAsync(int id);
        Task<HeroSlide?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug, int? excludeId);
        Task<int> CountFeaturedAsync(int? excludeId);
        Task<int> GetNextFeaturedOrderAsync();
        Task<T> RunInSerializableTransactionAsync<T>(Func<Task<T>> operation);
        Task<HeroSlide> CreateAsync(HeroSlide slide);
        Task<HeroSlide?> UpdateAsync(int id, UpdateHeroSlideDto dto);
        Task<bool> DeleteAsync(int id);
    }
}