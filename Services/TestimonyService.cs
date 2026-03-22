using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services
{
    public class TestimonyService : ITestimonyService
    {
        private readonly ITestimonyRepository _testimonyRepo;
        public TestimonyService(ITestimonyRepository testimonyRepo)
        {
            _testimonyRepo = testimonyRepo;
        }

        public async Task<TestimonyResponseDto> CreateAsync(CreateTestimonyDto createDto, string? name, string? appUserId)
        {
            var testimony = createDto.ToTestimonyFromCreateDto(name, appUserId);

            var created = await _testimonyRepo.CreateAsync(testimony);

            return created.ToTestimonyResponseDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _testimonyRepo.DeleteAsync(id);
        }

        public async Task<PagedResult<TestimonyResponseDto>> GetAllAsync(TestimonyQueryObject query)
        {
            var testimonies = await _testimonyRepo.GetAllAsync(query);
            var totalCount = await _testimonyRepo.GetAllCountAsync(query);

            return new PagedResult<TestimonyResponseDto>
            {
                Items = testimonies.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<PagedResult<TestimonyResponseDto>> GetApprovedAsync(TestimonyQueryObject query)
        {
            var testimonies = await _testimonyRepo.GetApprovedAsync(query);
            var totalCount = await _testimonyRepo.GetApprovedCountAsync(query);

            return new PagedResult<TestimonyResponseDto>
            {
                Items = testimonies.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<TestimonyResponseDto?> GetByIdAsync(int id)
        {
            var testimony = await _testimonyRepo.GetByIdAsync(id);

            if (testimony is null)
                return null;

            return testimony.ToTestimonyResponseDto();
        }

        public async Task<TestimonyResponseDto?> UpdateStatusAsync(int id, UpdateTestimonyDto updateDto)
        {
            var updated = await _testimonyRepo.UpdateStatusAsync(id, updateDto);

            if (updated is null)
                return null;

            return updated.ToTestimonyResponseDto();
        }
    }
}