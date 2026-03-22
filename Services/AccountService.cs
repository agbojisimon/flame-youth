using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.User;
using GlobalFlameMinistry.API.Interfaces.Account;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GlobalFlameMinistry.API.Services
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
            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("Invalid token.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new UnauthorizedAccessException("User no longer exists.");

            var roles = await _userManager.GetRolesAsync(user);

            return user.ToUserDto(roles.ToList());
        }

        public async Task<UserDto> UpdateMyProfileAsync(ClaimsPrincipal principal, UpdateUserDto dto)
        {
            var user = await _userManager.GetUserAsync(principal);

            if (user == null)
                throw new UnauthorizedAccessException("User not found.");

            user.ApplyUpdate(dto);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            var roles = await _userManager.GetRolesAsync(user);
            return user.ToUserDto(roles.ToList());
        }

        public async Task<string> ChangePasswordAsync(ClaimsPrincipal principal, ChangePasswordDto dto)
        {
            var appUser = await _userManager.GetUserAsync(principal);

            if (appUser == null)
                throw new UnauthorizedAccessException("User not found.");

            var result = await _userManager.ChangePasswordAsync(
                appUser,
                dto.CurrentPassword,
                dto.NewPassword
            );

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException(errors);
            }

            return "Password changed successfully.";
        }
    }
}