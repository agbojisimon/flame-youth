using g_flame_youth.DTOs.PrayerRequest;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;

namespace g_flame_youth.Services
{
    public class PrayerRequestService : IPrayerRequestService
    {
        private readonly IPrayerRequestRepository _prayerRepo;
        public PrayerRequestService(IPrayerRequestRepository prayerRepo)
        {
            _prayerRepo = prayerRepo;
        }
        public async Task<PrayerRequestResponseDto> CreatePrayerAsync(CreatePrayerDto createDto)
        {
            var prayer = createDto.ToPrayerRequestFromCreateDto();
            prayer.CreatedAt = DateTime.UtcNow;

            await _prayerRepo.CreatePrayerAsync(prayer);

            return prayer.ToPrayerRequestResponseDto();
        }

        public async Task<bool> DeletePrayerAsync(int id)
        {
            var prayer = _prayerRepo.GetByIdAsync(id);

            if (prayer == null)
                return false;

            return await _prayerRepo.DeleteAsync(id);
        }

        public async Task<PrayerRequestResponseDto?> GetByIdAsync(int id)
        {
            var prayer = await _prayerRepo.GetByIdAsync(id);

            if (prayer == null)
                return null;

            return prayer.ToPrayerRequestResponseDto();
        }

        public async Task<List<PrayerRequestResponseDto>> GetPrayerRequestsAsync(PrayerReqeustQueryObject query)
        {
            var prayers = await _prayerRepo.GetPrayerRequestsAsync(query);

            return prayers.Select(p => p.ToPrayerRequestResponseDto()).ToList();
        }
    }
}