using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;

namespace g_flame_youth.Interfaces
{
    public interface IAnnouncementService
    {
        Task<List<AnnouncementDto>> GetAnnouncementsAsync(AnnouncementQueryObject query);
        Task<AnnouncementDto?> GetAnnouncementByIdAsync(int Id);
        Task<AnnouncementDto> CreateAnnouncementAsync(CreateAnnouncementDto createDto, string userId);
        Task<AnnouncementDto?> UpdateAnnouncementAsync(int Id, UpdateAnnouncementDto updateDto);
        Task<bool> DeleteAnnouncementAsync(int Id);
    }
}