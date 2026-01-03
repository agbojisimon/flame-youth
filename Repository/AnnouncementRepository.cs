using g_flame_youth.Data;
using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Models;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Repository
{
    public class AnnouncementRepository : IAnnouncementRepository
    {
        private readonly AppDbContext _context;
        public AnnouncementRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task CreateAnnouncementAsync(Announcement announcement)
        {
            await _context.Announcements.AddAsync(announcement);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAnnouncementAsync(int Id)
        {
            var announcement = await _context.Announcements.FirstOrDefaultAsync(a => a.Id == Id && !a.IsDeleted);

            if (announcement == null)
                return false;

            announcement.IsDeleted = true;
            announcement.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Announcement?> GetAnnouncementByIdAsync(int Id)
        {
            return await _context.Announcements.Include(a => a.CreatedBy).FirstOrDefaultAsync(a => a.Id == Id && !a.IsDeleted);
        }

        public async Task<List<Announcement>> GetAnnouncementsAsync(AnnouncementQueryObject query)
        {
            var announcements = _context.Announcements.Include(a => a.CreatedBy).Where(a => !a.IsDeleted).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Title))
            {
                announcements = announcements.Where(a => a.Title.Contains(query.Title));
            }
            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                announcements = query.SortBy.Equals("CreatedOn", StringComparison.OrdinalIgnoreCase) ? (query.IsDescending ? announcements.OrderByDescending(a => a.CreatedOn) : announcements.OrderBy(a => a.CreatedOn)) : announcements;
            }
            else
            {
                announcements = announcements.OrderByDescending(a => a.CreatedOn);
            }

            var skip = (query.PageNumber - 1) * query.PageSize;

            return await announcements.Skip(skip).Take(query.PageSize).ToListAsync();
        }

        public async Task UpdateAnnouncementAsync(Announcement announcement)
        {
            _context.Announcements.Update(announcement);
            await _context.SaveChangesAsync();
        }
    }
}