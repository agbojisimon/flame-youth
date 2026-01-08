
using g_flame_youth.DTOs.Devotional;
using g_flame_youth.Helpers.Queries;
using g_flame_youth.Migrations;

namespace g_flame_youth.Interfaces
{
    public interface IDevotionalService
    {
        Task<DevotionalResponseDto?> GetTodayDevotionalAsync();
        Task<DevotionalResponseDto?> GetDevotionalByIdAsync(int id);
        Task<List<DevotionalResponseDto>> GetDevotionalsAsync(DevotionalQueryObject query);
        Task<List<DevotionalResponseDto>> PreviewDevotionalsAsync(DevotionalQueryObject query);
        Task<DevotionalResponseDto> CreateDevotionalAsync(CreateDevotionalDto createDto);
        Task<DevotionalResponseDto> UpdateDevotionalAsync(int id, UpdateDevotionalDto updateDto);
        Task<List<DevotionalResponseDto>> GetPublishedDevotionalsAsync(DevotionalQueryObject query);

        Task<DevotionalResponseDto> PublishDevotionalAsync(int id);
        Task<bool> DeleteDevotionalAsync(int id);
    }
}