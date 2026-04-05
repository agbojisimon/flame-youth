using GlobalFlameMinistry.API.DTOs.Donation;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IDonationService
    {
        Task<InitiateDonationResponseDto> InitiatePaystackAsync(InitiateDonationDto dto);
        Task<InitiateDonationResponseDto> InitiateFlutterwaveAsync(InitiateDonationDto dto);
        Task<bool> VerifyPaystackAsync(string reference);
        Task<bool> VerifyFlutterwaveAsync(string transactionId);
    }
}