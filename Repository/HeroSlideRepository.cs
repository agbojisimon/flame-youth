using System.Data;
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
                .Where(h => h.IsActive && h.MediaType == "Image")
                .OrderBy(h => h.DisplayOrder)
                .ThenBy(h => h.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<HeroSlide>> GetClipsAsync()
        {
            return await _context.HeroSlides
                .Where(h => h.IsActive && h.MediaType == "Video")
                .OrderBy(h => h.DisplayOrder)
                .ThenBy(h => h.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<HeroSlide>> GetFeaturedClipsAsync()
        {
            return await _context.HeroSlides
                .Where(h => h.IsActive && h.MediaType == "Video" && h.IsFeatured)
                .OrderBy(h => h.FeaturedOrder)
                .ThenBy(h => h.CreatedOn)
                .Take(3)
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

        public async Task<int> CountFeaturedAsync(int? excludeId)
        {
            var query = _context.HeroSlides
                .Where(h => h.IsActive && h.MediaType == "Video" && h.IsFeatured);

            if (excludeId.HasValue)
                query = query.Where(h => h.Id != excludeId.Value);

            return await query.CountAsync();
        }

        public async Task<int> GetNextFeaturedOrderAsync()
        {
            var used = await _context.HeroSlides
                .Where(h => h.IsActive && h.MediaType == "Video" && h.IsFeatured)
                .Select(h => h.FeaturedOrder)
                .ToListAsync();

            var order = 0;
            while (used.Contains(order))
                order++;

            return order;
        }

        public async Task<T> RunInSerializableTransactionAsync<T>(Func<Task<T>> operation)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var result = await operation();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
            slide.IsFeatured = false;
            slide.FeaturedOrder = 0;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
