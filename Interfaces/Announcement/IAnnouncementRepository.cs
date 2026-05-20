using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IAnnouncementRepository
    {
        Task<List<Announcement>> GetAllAsync(AnnouncementQueryObject query);
        Task<int> GetCountAsync(AnnouncementQueryObject query);
        Task<Announcement?> GetByIdAsync(int id);
        Task<Announcement?> GetBySlugAsync(string slug);
        Task<Announcement> CreateAsync(Announcement announcement);
        Task<Announcement?> UpdateAsync(int id, UpdateAnnouncementDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}