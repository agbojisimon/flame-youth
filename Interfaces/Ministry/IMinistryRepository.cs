using GlobalFlameMinistry.API.DTOs.Ministry;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces.Ministry
{
    public interface IMinistryRepository
    {
        Task<List<MinistryDepartment>> GetAllAsync(MinistryQueryObject query);
        Task<int> GetCountAsync(MinistryQueryObject query);
        Task<MinistryDepartment?> GetByIdAsync(int id);
        Task<MinistryDepartment?> GetBySlugAsync(string slug);
        Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
        Task<MinistryDepartment> CreateAsync(MinistryDepartment ministry);
        Task<MinistryDepartment?> UpdateAsync(int id, UpdateMinistryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}