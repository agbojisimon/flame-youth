using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Donation;
using GlobalFlameMinistry.API.Helpers.Queries;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IAdminDonationService
    {
        Task<PagedResult<DonationResponseDto>> GetAllAsync(DonationQueryObject query);
        Task<DonationResponseDto?> GetByIdAsync(int id);
        Task<DonationStatsDto> GetStatsAsync();
    }
}