using System.Security.Claims;
using g_flame_youth.DTOs.User;

namespace g_flame_youth.Interfaces.Account
{
    public interface IAccountService
    {
        Task<UserDto> GetMyProfileAsync(ClaimsPrincipal user);
        Task<UserDto> UpdateMyProfileAsync(ClaimsPrincipal user, UpdateUserDto dto);
    }
}