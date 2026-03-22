using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;
namespace GlobalFlameMinistry.API.Interfaces
{
    public interface ITestimonyRepository
    {
        Task<List<Testimony>> GetApprovedAsync(TestimonyQueryObject query);
        Task<int> GetApprovedCountAsync(TestimonyQueryObject query);
        Task<List<Testimony>> GetAllAsync(TestimonyQueryObject query);
        Task<int> GetAllCountAsync(TestimonyQueryObject query);
        Task<Testimony?> GetByIdAsync(int id);
        Task<Testimony> CreateAsync(Testimony testimony);
        Task<Testimony?> UpdateStatusAsync(int id, UpdateTestimonyDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}