using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
  public class AnnouncementService : IAnnouncementService
  {
    private readonly IAnnouncementRepository _announceRepo;
    public AnnouncementService(IAnnouncementRepository announceRepo)
    {
      _announceRepo = announceRepo;
    }

    public async Task<AnnouncementDto> CreateAsync(CreateAnnouncementDto dto, string createdById)
    {
      var announcement = dto.ToAnnouncementFromCreateDto(createdById);

      var created = await _announceRepo.CreateAsync(announcement);

      return created.ToAnnouncementDto();
    }

    public async Task<bool> DeleteAsync(int id)
    {
      return await _announceRepo.DeleteAsync(id);
    }

    public async Task<PagedResult<AnnouncementDto>> GetAllAsync(AnnouncementQueryObject query)
    {
      var announcements = await _announceRepo.GetAllAsync(query);
      var totalCount = await _announceRepo.GetCountAsync(query);

      return new PagedResult<AnnouncementDto>
      {
        Items = announcements.ToDtoList(),
        TotalCount = totalCount,
        PageNumber = query.PageNumber,
        PageSize = query.PageSize
      };
    }

    public async Task<AnnouncementDto?> GetByIdAsync(int id)
    {
      var announcement = await _announceRepo.GetByIdAsync(id);

      // Return null if not found 
      if (announcement is null) return null;

      return announcement.ToAnnouncementDto();
    }

    public async Task<AnnouncementDto?> UpdateAsync(int id, UpdateAnnouncementDto dto)
    {
      var updated = await _announceRepo.UpdateAsync(id, dto);

      if (updated is null) return null;

      return updated.ToAnnouncementDto();
    }
  }
}