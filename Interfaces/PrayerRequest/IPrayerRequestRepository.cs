using g_flame_youth.Helpers;
using g_flame_youth.Models;

namespace g_flame_youth.Interfaces
{
    public interface IPrayerRequestRepository
    {
        Task<List<PrayerRequest>> GetPrayerRequestsAsync(PrayerReqeustQueryObject qeury);
        Task<PrayerRequest?> GetByIdAsync(int id);
        Task CreatePrayerAsync(PrayerRequest request);
        Task<bool> DeleteAsync(int id);
    }
}