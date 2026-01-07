using g_flame_youth.DTOs.PrayerRequest;
using g_flame_youth.Helpers;

namespace g_flame_youth.Interfaces
{
    public interface IPrayerRequestService
    {
        Task<List<PrayerRequestResponseDto>> GetPrayerRequestsAsync(PrayerReqeustQueryObject query);
        Task<PrayerRequestResponseDto?> GetByIdAsync(int id);
        Task<PrayerRequestResponseDto> CreatePrayerAsync(CreatePrayerDto createDto);
        Task<bool> DeletePrayerAsync(int id);
    }
}