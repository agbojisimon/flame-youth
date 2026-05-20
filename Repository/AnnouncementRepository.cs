using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;
namespace GlobalFlameMinistry.API.Repository
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly AppDbContext _context;
        public AnnouncementRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Announcement> CreateAsync(Announcement announcement)
        {
            await _context.Announcements.AddAsync(announcement);
            await _context.SaveChangesAsync();
            // Update slug to include ID
            announcement.Slug = GlobalFlameMinistry.API.Helpers.SlugHelper.Generate(announcement.Title, announcement.Id);
            await _context.SaveChangesAsync();

            return announcement;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement is null) return false;

            // Soft delete
            announcement.IsDeleted = true;
            announcement.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Announcements.AnyAsync(a => a.Id == id);
        }

        public async Task<List<Announcement>> GetAllAsync(AnnouncementQueryObject query)
        {
            var announcements = _context.Announcements.AsQueryable();

            // Filter by title
            if (!string.IsNullOrWhiteSpace(query.Title))
                announcements = announcements.Where(a =>
                    a.Title.ToLower().Contains(query.Title.ToLower()));

            // Filter by module
            if (!string.IsNullOrWhiteSpace(query.Module))
                announcements = announcements.Where(a => a.Module == query.Module);

            // Filter by category
            if (!string.IsNullOrWhiteSpace(query.Category))
                announcements = announcements.Where(a => a.Category == query.Category);

            // Filter by published status
            if (query.IsPublished.HasValue)
                announcements = announcements.Where(a => a.IsPublished == query.IsPublished.Value);

            // Filter by Date
            if (query.FromDate.HasValue)
                announcements = announcements.Where(a => a.CreatedOn >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                announcements = announcements.Where(a => a.CreatedOn <= query.ToDate.Value);

            announcements = query.SortBy?.ToLower() switch
            {
                "title" => query.IsDescending
                    ? announcements.OrderByDescending(a => a.Title)
                    : announcements.OrderBy(a => a.Title),

                "createdon" => query.IsDescending
                    ? announcements.OrderByDescending(a => a.CreatedOn)
                    : announcements.OrderBy(a => a.CreatedOn),

                "category" => query.IsDescending
                    ? announcements.OrderByDescending(a => a.Category)
                    : announcements.OrderBy(a => a.Category),
                _ => announcements.OrderByDescending(a => a.CreatedOn)
            };

            return await announcements.Skip((query.PageNumber - 1) * query.PageSize).Take(query.PageSize).ToListAsync();
        }

        public async Task<Announcement?> GetByIdAsync(int id)
        {
            return await _context.Announcements.FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Announcement?> GetBySlugAsync(string slug)
        {
            return await _context.Announcements.FirstOrDefaultAsync(a => a.Slug == slug);
        }

        public async Task<int> GetCountAsync(AnnouncementQueryObject query)
        {
            var announcements = _context.Announcements.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Title))
                announcements = announcements.Where(a =>
                    a.Title.ToLower().Contains(query.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Module))
                announcements = announcements.Where(a => a.Module == query.Module);

            if (!string.IsNullOrWhiteSpace(query.Category))
                announcements = announcements.Where(a => a.Category == query.Category);

            if (query.IsPublished.HasValue)
                announcements = announcements.Where(a => a.IsPublished == query.IsPublished.Value);

            if (query.FromDate.HasValue)
                announcements = announcements.Where(a => a.CreatedOn >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                announcements = announcements.Where(a => a.CreatedOn <= query.ToDate.Value);

            return await announcements.CountAsync();
        }

        public async Task<Announcement?> UpdateAsync(int id, UpdateAnnouncementDto updateDto)
        {
            var announcement = await _context.Announcements.FindAsync(id);

            if (announcement is null) return null;

            // ApplyUpdate is the mapper extension method — keeps this clean
            announcement.ApplyUpdate(updateDto);

            await _context.SaveChangesAsync();

            // Refresh slug in case title changed
            announcement.Slug = GlobalFlameMinistry.API.Helpers.SlugHelper.Generate(announcement.Title, announcement.Id);
            await _context.SaveChangesAsync();

            return announcement;
        }
    }
}