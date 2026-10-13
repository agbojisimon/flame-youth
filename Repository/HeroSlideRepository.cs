using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class HeroSlideRepository : IHeroSlideRepository
    {
        private readonly AppDbContext _context;

        public HeroSlideRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<HeroSlide>> GetActiveAsync()
        {
            return await _context.HeroSlides
                .Where(h => h.IsActive)
                .OrderBy(h => h.DisplayOrder)
                .ThenBy(h => h.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<HeroSlide>> GetAllAsync()
        {
            return await _context.HeroSlides
                .OrderBy(h => h.DisplayOrder)
                .ThenBy(h => h.CreatedOn)
                .ToListAsync();
        }

        public async Task<HeroSlide?> GetByIdAsync(int id)
        {
            return await _context.HeroSlides.FindAsync(id);
        }

        public async Task<HeroSlide> CreateAsync(HeroSlide slide)
        {
            await _context.HeroSlides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return slide;
        }

        public async Task<HeroSlide?> UpdateAsync(int id, UpdateHeroSlideDto dto)
        {
            var slide = await _context.HeroSlides.FindAsync(id);
            if (slide is null) return null;

            slide.ApplyUpdate(dto);
            await _context.SaveChangesAsync();
            return slide;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var slide = await _context.HeroSlides.FindAsync(id);
            if (slide is null) return false;

            slide.IsDeleted = true;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}