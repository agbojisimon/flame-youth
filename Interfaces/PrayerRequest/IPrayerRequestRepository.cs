using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IPrayerRequestRepository
    {
        Task<List<PrayerRequest>> GetAllAsync(PrayerRequestQueryObject query);
        Task<int> GetCountAsync(PrayerRequestQueryObject query);
        Task<PrayerRequest?> GetByIdAsync(int id);
        Task<PrayerRequest?> GetByTokenAsync(string token);
        Task<PrayerRequest> CreateAsync(PrayerRequest request);
        Task<PrayerRequest?> UpdateAsync(int id, UpdatePrayerRequestDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}