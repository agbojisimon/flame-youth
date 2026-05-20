using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IAnnouncementService
    {
        Task<PagedResult<AnnouncementDto>> GetAllAsync(AnnouncementQueryObject query);
        Task<AnnouncementDto?> GetByIdAsync(int id);
        Task<AnnouncementDto?> GetBySlugAsync(string slug);
        Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto, string createdById);
        Task<AnnouncementDto?> UpdateAsync(int id, UpdateAnnouncementDto dto);
        Task<bool> DeleteAsync(int id);
    }
}