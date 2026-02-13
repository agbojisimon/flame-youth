using System.Security.Claims;
using g_flame_youth.DTOs.User;
using g_flame_youth.Interfaces.Account;
using g_flame_youth.Mappers;
using g_flame_youth.Models;
using Microsoft.AspNetCore.Identity;

namespace g_flame_youth.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<AppUser> _userManager;

        public AccountService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserDto> GetMyProfileAsync(ClaimsPrincipal principal)
        {
            if (principal?.Identity == null || !principal.Identity.IsAuthenticated)
                throw new UnauthorizedAccessException("User is not authenticated.");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token: user ID claim missing.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UnauthorizedAccessException("User no longer exists.");

            return user.ToUserDto();
        }

        public async Task<UserDto> UpdateMyProfileAsync(ClaimsPrincipal principal, UpdateUserDto dto)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            user = dto.ToAppUser(user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                throw new InvalidOperationException("Profile update failed");

            return user.ToUserDto();
        }
    }
}