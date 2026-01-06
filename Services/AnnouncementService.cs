using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;

namespace g_flame_youth.Services
{
  public class AnnouncementService : IAnnouncementService
  {
    private readonly IAnnouncementRepository _announceRepo;
    public AnnouncementService(IAnnouncementRepository announceRepo)
    {
      _announceRepo = announceRepo;
    }
    public async Task<AnnouncementDto> CreateAnnouncementAsync(CreateAnnouncementDto createDto, string userId)
    {
      var announcement = createDto.ToAnnouncementFromCreateDto();
      announcement.CreatedById = userId;
      announcement.CreatedOn = DateTime.UtcNow;

      await _announceRepo.CreateAnnouncementAsync(announcement);

      return announcement.ToAnnouncementDto();
    }

    public async Task<bool> DeleteAnnouncementAsync(int Id)
    {
      var announcement = await _announceRepo.GetAnnouncementByIdAsync(Id);

      if (announcement == null)
        return false;

      return await _announceRepo.DeleteAnnouncementAsync(Id);
    }

    public async Task<AnnouncementDto?> GetAnnouncementByIdAsync(int Id)
    {
      var announcement = await _announceRepo.GetAnnouncementByIdAsync(Id);

      if (announcement == null)
        return null;

      return announcement.ToAnnouncementDto();
    }

    public async Task<List<AnnouncementDto>> GetAnnouncementsAsync(AnnouncementQueryObject query)
    {
      var announcements = await _announceRepo.GetAnnouncementsAsync(query);

      return announcements.Select(a => a.ToAnnouncementDto()).ToList();
    }

    public async Task<AnnouncementDto?> UpdateAnnouncementAsync(int Id, UpdateAnnouncementDto updateDto)
    {
      var announcement = await _announceRepo.GetAnnouncementByIdAsync(Id);

      if (announcement == null)
        return null;

      announcement.Title = updateDto.Title;
      announcement.Content = updateDto.Content;
      announcement.Category = updateDto.Category;
      announcement.UpdatedOn = DateTime.UtcNow;

      await _announceRepo.UpdateAnnouncementAsync(announcement);

      return announcement.ToAnnouncementDto();
    }
  }
}