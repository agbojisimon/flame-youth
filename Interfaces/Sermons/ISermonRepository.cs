using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface ISermonRepository
    {
        Task<List<Sermon>> GetAllAsync(SermonQueryObject query);
        Task<int> GetCountAsync(SermonQueryObject query);
        Task<Sermon?> GetByIdAsync(int id);
        Task<Sermon?> GetBySlugAsync(string slug);
        Task<Sermon> CreateAsync(Sermon sermon);
        Task<Sermon?> UpdateAsync(int id, UpdateSermonDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}