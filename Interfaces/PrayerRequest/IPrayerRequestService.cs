using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IPrayerRequestService
    {
        Task<PagedResult<PrayerRequestResponseDto>> GetAllAsync(PrayerRequestQueryObject query);
        Task<PrayerRequestResponseDto?> GetByIdAsync(int id);
        Task<PrayerRequestResponseDto> CreateAsync(CreatePrayerDto dto, string? name, string? email, string? appUserId);
        Task<PrayerRequestResponseDto?> GetByTokenAsync(string token);
        Task<PrayerRequestResponseDto?> MarkAsAttendedAsync(int id, UpdatePrayerRequestDto dto);
    }
}