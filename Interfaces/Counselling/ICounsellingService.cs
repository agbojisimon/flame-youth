using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Counselling;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces.Counselling
{
    public interface ICounsellingService
    {
        Task<PagedResult<CounsellingResponseDto>> GetAllAsync(CounsellingQueryObject query);
        Task<CounsellingResponseDto?> GetByIdAsync(int id);
        Task<CounsellingResponseDto> CreateAsync(CreateCounsellingRequestDto dto, string? appUserId);
        Task<CounsellingResponseDto?> AssignAsync(int id, AssignCounsellorDto dto);
        Task<CounsellingResponseDto?> UpdateStatusAsync(int id, CounsellingStatus status);
        Task<bool> DeleteAsync(int id);
    }
}