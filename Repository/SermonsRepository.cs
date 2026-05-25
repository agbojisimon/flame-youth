using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repositories
{
    public class SermonRepository : ISermonRepository
    {
        private readonly AppDbContext _context;

        public SermonRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sermon>> GetAllAsync(SermonQueryObject query)
        {
            var sermons = _context.Sermons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Title))
                sermons = sermons.Where(s =>
                    s.Title.ToLower().Contains(query.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Speaker))
                sermons = sermons.Where(s =>
                    s.Speaker.ToLower().Contains(query.Speaker.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Series))
                sermons = sermons.Where(s =>
                    s.Series.ToLower().Contains(query.Series.ToLower()));

            if (query.IsPublished.HasValue)
                sermons = sermons.Where(s =>
                    s.IsPublished == query.IsPublished.Value);

            if (query.IsFeatured.HasValue)
                sermons = sermons.Where(s =>
                    s.IsFeatured == query.IsFeatured.Value);

            if (query.FromDate.HasValue)
                sermons = sermons.Where(s =>
                    s.SermonDate >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                sermons = sermons.Where(s =>
                    s.SermonDate <= query.ToDate.Value);

            sermons = query.SortBy?.ToLower() switch
            {
                "title" => query.IsDescending
                    ? sermons.OrderByDescending(s => s.Title)
                    : sermons.OrderBy(s => s.Title),

                "speaker" => query.IsDescending
                    ? sermons.OrderByDescending(s => s.Speaker)
                    : sermons.OrderBy(s => s.Speaker),

                "sermondate" => query.IsDescending
                    ? sermons.OrderByDescending(s => s.SermonDate)
                    : sermons.OrderBy(s => s.SermonDate),

                // Default — newest sermon first
                _ => sermons.OrderByDescending(s => s.SermonDate)
            };

            return await sermons
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(SermonQueryObject query)
        {
            var sermons = _context.Sermons.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Title))
                sermons = sermons.Where(s =>
                    s.Title.ToLower().Contains(query.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Speaker))
                sermons = sermons.Where(s =>
                    s.Speaker.ToLower().Contains(query.Speaker.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Series))
                sermons = sermons.Where(s =>
                    s.Series.ToLower().Contains(query.Series.ToLower()));

            if (query.IsPublished.HasValue)
                sermons = sermons.Where(s =>
                    s.IsPublished == query.IsPublished.Value);

            if (query.IsFeatured.HasValue)
                sermons = sermons.Where(s =>
                    s.IsFeatured == query.IsFeatured.Value);

            if (query.FromDate.HasValue)
                sermons = sermons.Where(s =>
                    s.SermonDate >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                sermons = sermons.Where(s =>
                    s.SermonDate <= query.ToDate.Value);

            return await sermons.CountAsync();
        }

        public async Task<Sermon?> GetByIdAsync(int id)
            => await _context.Sermons.FirstOrDefaultAsync(s => s.Id == id);

        public async Task<Sermon?> GetBySlugAsync(string slug)
            => await _context.Sermons.FirstOrDefaultAsync(s => s.Slug == slug);

        public async Task<Sermon> CreateAsync(Sermon sermon)
        {
            await _context.Sermons.AddAsync(sermon);
            await _context.SaveChangesAsync();

            // Update slug to include ID for uniqueness/readability
            sermon.Slug = GlobalFlameMinistry.API.Helpers.SlugHelper.Generate(sermon.Title, sermon.Id);
            await _context.SaveChangesAsync();

            return sermon;
        }

        public async Task<Sermon?> UpdateAsync(int id, UpdateSermonDto dto)
        {
            var sermon = await _context.Sermons.FindAsync(id);

            if (sermon is null)
                return null;

            sermon.ApplyUpdate(dto);
            await _context.SaveChangesAsync();

            // If title changed, ensure slug is updated to reflect new title and id
            sermon.Slug = GlobalFlameMinistry.API.Helpers.SlugHelper.Generate(sermon.Title, sermon.Id);
            await _context.SaveChangesAsync();

            return sermon;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sermon = await _context.Sermons.FindAsync(id);
            if (sermon is null)
                return false;

            _context.Sermons.Remove(sermon);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
            => await _context.Sermons.AnyAsync(s => s.Id == id);

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
    }
}