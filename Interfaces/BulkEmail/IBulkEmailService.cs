using GlobalFlameMinistry.API.DTOs.BulkEmail;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces.BulkEmail
{
    public interface IBulkEmailService
    {
        Task<BulkEmailResponseDto> SendNowAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName);

        Task<BulkEmailResponseDto> ScheduleAsync(
            SendBulkEmailDto dto, string adminUserId, string adminName);

        Task<PagedResult<BulkEmailResponseDto>> GetHistoryAsync(
            BulkEmailQueryObject query);

        Task<BulkEmailStatsDto> GetStatsAsync();
        Task<bool> CancelScheduledAsync(int id);
        Task ProcessScheduledEmailsAsync();
    }
}