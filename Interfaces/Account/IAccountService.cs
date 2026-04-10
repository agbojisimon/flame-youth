using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.Youth;

namespace GlobalFlameMinistry.API.Interfaces.Account
{
    public interface IAccountService
    {
        Task<MyProfileDto?> GetProfileAsync(string userId);
        Task<MyProfileDto?> UpdateProfileAsync(string userId, UpdateProfileDto dto);
        Task<string?> UpdateProfilePictureAsync(string userId, string profilePictureUrl);
        Task<List<MyPrayerRequestDto>> GetMyPrayerRequestsAsync(string userId);
        Task<List<MyRegistrationDto>> GetMyRegistrationsAsync(string userId);
        Task<List<MyDonationDto>> GetMyDonationsAsync(string userId);
        Task RequestEmailChangeAsync(string userId, string newEmail);
        Task<bool> ConfirmEmailChangeAsync(string userId, ConfirmEmailChangeDto dto);
        // IAccountService.cs
        Task<JoinYouthResultDto> JoinYouthCommunityAsync(string userId, JoinYouthDto dto);
    }
}