using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<List<Announcement>> GetAnnouncementsAsync(AnnouncementQueryObject query);
        Task<Announcement?> GetAnnouncementByIdAsync(int Id);
        Task CreateAnnouncementAsync(Announcement announcement);
        Task UpdateAnnouncementAsync(Announcement announcement);
        Task<bool> DeleteAnnouncementAsync(int Id);
    }
}