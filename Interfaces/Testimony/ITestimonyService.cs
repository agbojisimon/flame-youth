using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface ITestimonyService
    {
        Task<PagedResult<TestimonyResponseDto>> GetApprovedAsync(TestimonyQueryObject query);
        Task<PagedResult<TestimonyResponseDto>> GetAllAsync(TestimonyQueryObject query);
        Task<TestimonyResponseDto?> GetByIdAsync(int id);
        Task<TestimonyResponseDto> CreateAsync(CreateTestimonyDto dto, string? name, string? appUserId);
        Task<TestimonyResponseDto?> UpdateStatusAsync(int id, UpdateTestimonyDto updateDto);
        Task<bool> DeleteAsync(int id);
    }
}