using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<List<Announcement>> GetAnnouncementsAsync(AnnouncementQueryObject query);
        Task<Announcement?> GetAnnouncementByIdAsync(int Id);
        Task<Announcement> CreateAnnouncementAsync(Announcement announcement);
        Task<Announcement?> UpdateAnnouncementAsync(int Id, UpdateAnnouncementDto updateAnnouncementDto);
        Task<bool> DeleteAnnouncementAsync(int Id);
    }
}