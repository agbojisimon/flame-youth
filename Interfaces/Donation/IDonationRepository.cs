using GlobalFlameMinistry.API.DTOs.Donation;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IDonationRepository
    {
        Task<(IEnumerable<Donation> Items, int TotalCount)> GetAllAsync(DonationQueryObject query);
        Task<Donation?> GetByIdAsync(int id);
        Task<(decimal TotalAmount, int Completed, int Pending)> GetSummaryAsync();
        Task<DonationStatsDto> GetStatsAsync();
    }
}