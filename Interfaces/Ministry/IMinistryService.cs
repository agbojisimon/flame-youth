using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces.Ministry
{
    public interface IMinistryService
    {
        Task<PagedResult<MinistryResponseDto>> GetAllAsync(MinistryQueryObject query);
        Task<MinistryResponseDto?> GetByIdAsync(int id);
        Task<MinistryResponseDto?> GetBySlugAsync(string slug);
        Task<MinistryResponseDto> CreateAsync(CreateMinistryDto dto);
        Task<MinistryResponseDto?> UpdateAsync(int id, UpdateMinistryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}