using g_flame_youth.Data;
using g_flame_youth.DTOs.Announcement;
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
        public async Task<Announcement> CreateAnnouncementAsync(Announcement announcement)
        {
            await _context.Announcements.AddAsync(announcement);
            await _context.SaveChangesAsync();
            return announcement;
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

        public async Task<List<Announcement>> GetAnnouncementsAsync()
        {
            return await _context.Announcements.Include(a => a.CreatedBy).Where(a => !a.IsDeleted && a.Status == "Published").OrderByDescending(a => a.CreatedOn).ToListAsync();
        }

        public async Task<Announcement?> UpdateAnnouncementAsync(int Id, UpdateAnnouncementDto updateAnnouncementDto)
        {
            var existingAnnouncement = await _context.Announcements
        .FirstOrDefaultAsync(a => a.Id == Id && !a.IsDeleted);

            if (existingAnnouncement == null)
                return null;

            existingAnnouncement.Title = updateAnnouncementDto.Title;
            existingAnnouncement.Content = updateAnnouncementDto.Content;
            existingAnnouncement.Status = updateAnnouncementDto.Status;
            existingAnnouncement.Category = updateAnnouncementDto.Category;
            existingAnnouncement.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Explicitly load the CreatedBy navigation property
            await _context.Entry(existingAnnouncement)
                          .Reference(a => a.CreatedBy)
                          .LoadAsync();

            return existingAnnouncement;
        }

        internal async Task GetAnnouncementById(int id)
        {
            throw new NotImplementedException();
        }
    }
}