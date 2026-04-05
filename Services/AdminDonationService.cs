using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.DTOs.Donation;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;

namespace GlobalFlameMinistry.API.Services.Admin
{
    public class AdminDonationService : IAdminDonationService
    {
        private readonly IDonationRepository _donationRepo;

        public AdminDonationService(IDonationRepository donationRepo)
        {
            _donationRepo = donationRepo;
        }

        // ── GET ALL ───────────────────────────────────────────────────────────
        public async Task<PagedResult<DonationResponseDto>> GetAllAsync(DonationQueryObject query)
        {
            var (items, totalCount) = await _donationRepo.GetAllAsync(query);

            return new PagedResult<DonationResponseDto>
            {
                Items = items.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            };
        }

        public async Task<DonationResponseDto?> GetByIdAsync(int id)
        {
            var donation = await _donationRepo.GetByIdAsync(id);

            return donation?.ToDto();
        }

        public async Task<DonationStatsDto> GetStatsAsync()
        {
            return await _donationRepo.GetStatsAsync();
        }
    }
}