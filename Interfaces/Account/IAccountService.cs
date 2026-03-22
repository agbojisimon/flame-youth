using System.Security.Claims;
using GlobalFlameMinistry.API.DTOs.User;

namespace GlobalFlameMinistry.API.Interfaces.Account
{
    public interface IAccountService
    {
        Task<UserDto> GetMyProfileAsync(ClaimsPrincipal user);
        Task<UserDto> UpdateMyProfileAsync(ClaimsPrincipal user, UpdateUserDto dto);
        Task<string> ChangePasswordAsync(ClaimsPrincipal user, ChangePasswordDto dto);
    }
}