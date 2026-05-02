using GlobalFlameMinistry.API.DTOs.Counselling;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces.Counselling
{
    public interface ICounsellingRepository
    {
        Task<List<CounsellingRequest>> GetAllAsync(CounsellingQueryObject query);
        Task<int> GetCountAsync(CounsellingQueryObject query);
        Task<CounsellingRequest?> GetByIdAsync(int id);
        Task<CounsellingRequest> CreateAsync(CounsellingRequest request);
        Task<CounsellingRequest?> UpdateStatusAsync(int id, CounsellingStatus status);
        Task<bool> DeleteAsync(int id);
    }
}