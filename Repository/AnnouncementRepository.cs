using g_flame_youth.Data;
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

        public async Task<Announcement?> GetAnnouncementByIdAsync(int Id)
        {
            return await _context.Announcements.FirstOrDefaultAsync(a => a.Id == Id && !a.IsDeleted);
        }

        public async Task<List<Announcement>> GetAnnouncementsAsync()
        {
            return await _context.Announcements.Where(a => a.IsDeleted && a.Status == "Published").OrderByDescending(a => a.CreatedOn).ToListAsync();

        }
    }
}