using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IHeroSlideRepository
    {
        Task<List<HeroSlide>> GetActiveAsync();
        Task<List<HeroSlide>> GetAllAsync();
        Task<HeroSlide?> GetByIdAsync(int id);
        Task<HeroSlide> CreateAsync(HeroSlide slide);
        Task<HeroSlide?> UpdateAsync(int id, UpdateHeroSlideDto dto);
        Task<bool> DeleteAsync(int id);
    }
}