using GlobalFlameMinistry.API.DTOs.BulkEmail;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces.BulkEmail
{
    public interface IBulkEmailRepository
    {
        Task<BulkEmailMessage> CreateAsync(BulkEmailMessage message);
        Task<BulkEmailMessage?> GetByIdAsync(int id);
        Task<List<BulkEmailMessage>> GetAllAsync(BulkEmailQueryObject query);
        Task<int> GetCountAsync(BulkEmailQueryObject query);
        Task<List<BulkEmailMessage>> GetDueScheduledAsync();
        Task<BulkEmailMessage?> UpdateAsync(BulkEmailMessage message);
        Task<bool> CancelAsync(int id);
        Task<BulkEmailStatsDto> GetStatsAsync();
    }
}