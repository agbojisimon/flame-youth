using GlobalFlameMinistry.API.DTOs.HeroSlide;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IHeroSlideService
    {
        Task<List<HeroSlideDto>> GetActiveAsync();
        Task<List<HeroSlideDto>> GetClipsAsync();
        Task<List<HeroSlideDto>> GetFeaturedClipsAsync();
        Task<List<HeroSlideDto>> GetAllAsync();
        Task<HeroSlideDto?> GetByIdAsync(int id);
        Task<HeroSlideDto> CreateAsync(CreateHeroSlideDto dto);
        Task<HeroSlideDto?> UpdateAsync(int id, UpdateHeroSlideDto dto);
        Task<bool> DeleteAsync(int id);
    }
}