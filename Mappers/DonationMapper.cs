using GlobalFlameMinistry.API.DTOs.Donation;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class DonationMapper
    {
        public static DonationResponseDto ToDto(this Donation donation)
        {
            return new DonationResponseDto
            {
                Id = donation.Id,
                DonorName = donation.DonorName,
                DonorEmail = donation.DonorEmail,
                Amount = donation.Amount,
                Currency = donation.Currency,
                TransactionReference = donation.TransactionReference,
                PaymentMethod = donation.PaymentMethod,
                Status = donation.Status,
                DonationType = donation.DonationType,
                SubaccountCode = donation.SubaccountCode,
                EventId = donation.EventId,
                EventTitle = donation.EventTitle,
                CreatedAt = donation.CreatedAt,
                UpdatedAt = donation.UpdatedAt,
            };
        }

        public static List<DonationResponseDto> ToDtoList(
            this IEnumerable<Donation> donations)
        {
            return donations.Select(d => d.ToDto()).ToList();
        }
    }
}