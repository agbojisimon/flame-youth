using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class PrayerRequestService : IPrayerRequestService
    {
        private readonly IPrayerRequestRepository _prayerRepo;
        public PrayerRequestService(IPrayerRequestRepository prayerRepo)
        {
            _prayerRepo = prayerRepo;
        }

        public async Task<PrayerRequestResponseDto> CreateAsync(CreatePrayerDto dto, string? name, string? email, string? appUserId)
        {
            var request = dto.ToPrayerFromCreateDto(name, email, appUserId);

            var created = await _prayerRepo.CreateAsync(request);

            return created.ToPrayerResponseDto();
        }

        public async Task<PagedResult<PrayerRequestResponseDto>> GetAllAsync(PrayerRequestQueryObject query)
        {
            var requests = await _prayerRepo.GetAllAsync(query);
            var totalCount = await _prayerRepo.GetCountAsync(query);

            return new PagedResult<PrayerRequestResponseDto>
            {
                Items = requests.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<PrayerRequestResponseDto?> GetByIdAsync(int id)
        {
            var request = await _prayerRepo.GetByIdAsync(id);

            if (request is null)
                return null;

            return request.ToPrayerResponseDto();
        }

        public async Task<PrayerRequestResponseDto?> GetByTokenAsync(string token)
        {
            var request = await _prayerRepo.GetByTokenAsync(token);

            if (request is null)
                return null;

            return request.ToPrayerResponseDto();
        }

        public async Task<PrayerRequestResponseDto?> MarkAsAttendedAsync(int id, UpdatePrayerRequestDto dto)
        {
            var updated = await _prayerRepo.UpdateAsync(id, dto);

            if (updated is null)
                return null;

            return updated.ToPrayerResponseDto();
        }
    }
}