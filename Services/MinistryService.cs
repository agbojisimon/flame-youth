using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces.Ministry;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class MinistryService : IMinistryService
    {
        private readonly IMinistryRepository _ministryRepo;

        public MinistryService(IMinistryRepository ministryRepo)
        {
            _ministryRepo = ministryRepo;
        }

        public async Task<PagedResult<MinistryResponseDto>> GetAllAsync(
            MinistryQueryObject query)
        {
            var ministries = await _ministryRepo.GetAllAsync(query);

            var total = await _ministryRepo.GetCountAsync(query);

            return new PagedResult<MinistryResponseDto>
            {
                Items = ministries.ToDtoList(),
                TotalCount = total,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            };
        }

        public async Task<MinistryResponseDto?> GetByIdAsync(int id)
        {
            var ministry = await _ministryRepo.GetByIdAsync(id);

            return ministry?.ToMinistryResponseDto();
        }

        public async Task<MinistryResponseDto?> GetBySlugAsync(string slug)
        {
            var ministry = await _ministryRepo.GetBySlugAsync(slug);

            return ministry?.ToMinistryResponseDto();
        }

        public async Task<MinistryResponseDto> CreateAsync(CreateMinistryDto dto)
        {
            var model = dto.ToModel();

            var created = await _ministryRepo.CreateAsync(model);

            return created.ToMinistryResponseDto();
        }

        public async Task<MinistryResponseDto?> UpdateAsync(int id, UpdateMinistryDto dto)
        {
            var updated = await _ministryRepo.UpdateAsync(id, dto);

            return updated?.ToMinistryResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _ministryRepo.DeleteAsync(id);
        }
    }
}