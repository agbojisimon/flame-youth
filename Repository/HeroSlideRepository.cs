using System.Data;
using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Helpers;
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
                .Include(h => h.ContentBlocks)
                .Where(h => h.IsActive && h.MediaType == "Video")
                .OrderBy(h => h.DisplayOrder)
                .ThenBy(h => h.CreatedOn)
                .ToListAsync();
        }

        public async Task<List<HeroSlide>> GetFeaturedClipsAsync()
        {
            return await _context.HeroSlides
                .Include(h => h.ContentBlocks)
                .Where(h => h.IsActive && h.MediaType == "Video" && h.IsFeatured)
                .OrderBy(h => h.FeaturedOrder)
                .ThenBy(h => h.CreatedOn)
                .Take(3)
                .ToListAsync();
        }

        public async Task<List<HeroSlide>> GetAllAsync()
        {
            return await _context.HeroSlides
                .Include(h => h.ContentBlocks)
                .OrderBy(h => h.DisplayOrder)
                .ThenBy(h => h.CreatedOn)
                .ToListAsync();
        }

        public async Task<HeroSlide?> GetByIdAsync(int id)
        {
            return await _context.HeroSlides
                .Include(h => h.ContentBlocks)
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<HeroSlide?> GetBySlugAsync(string slug)
        {
            return await _context.HeroSlides
                .Include(h => h.ContentBlocks)
                .FirstOrDefaultAsync(h =>
                    h.IsActive &&
                    h.MediaType == "Video" &&
                    h.Slug == slug);
        }

        public async Task<bool> SlugExistsAsync(string slug, int? excludeId)
        {
            var query = _context.HeroSlides.Where(h => h.Slug == slug);

            if (excludeId.HasValue)
                query = query.Where(h => h.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        private async Task<string> GenerateUniqueSlugAsync(string? desired, string? title, int? excludeId)
        {
            var source = !string.IsNullOrWhiteSpace(desired) ? desired : title ?? string.Empty;
            var baseSlug = SlugHelper.Generate(source);
            if (string.IsNullOrWhiteSpace(baseSlug))
                baseSlug = "clip";

            var slug = baseSlug;
            var suffix = 1;

            while (await SlugExistsAsync(slug, excludeId))
            {
                suffix++;
                slug = $"{baseSlug}-{suffix}";
            }

            return slug;
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
            if (IsVideo(slide.MediaType))
            {
                slide.Slug = await GenerateUniqueSlugAsync(
                    slide.Slug ?? slide.Title, slide.Title, excludeId: null);
            }
            else
            {
                slide.Slug = null;
            }

            await _context.HeroSlides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return slide;
        }

        public async Task<HeroSlide?> UpdateAsync(int id, UpdateHeroSlideDto dto)
        {
            var slide = await _context.HeroSlides
                .Include(h => h.ContentBlocks)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (slide is null) return null;

            slide.ApplyUpdate(dto);

            if (IsVideo(dto.MediaType))
            {
                var desired = string.IsNullOrWhiteSpace(dto.Slug) ? null : dto.Slug;
                slide.Slug = await GenerateUniqueSlugAsync(desired, dto.Title, excludeId: id);
            }
            else
            {
                slide.Slug = null;
            }

            // Replace the write-up blocks with the submitted set when provided
            // (null leaves them untouched so edits from the slide modal do not
            // wipe an existing write-up).
            if (dto.ContentBlocks is not null)
            {
                _context.HeroSlideBlocks.RemoveRange(slide.ContentBlocks);
                slide.ContentBlocks = dto.ContentBlocks.ToHeroSlideBlocks();
            }

            await _context.SaveChangesAsync();
            return slide;
        }

        private static bool IsVideo(string? mediaType)
            => string.Equals(mediaType, "Video", StringComparison.OrdinalIgnoreCase);

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
