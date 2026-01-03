using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.User;
using g_flame_youth.Helpers;

namespace g_flame_youth.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetUsersAsync(UserQueryObject query);
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<UserDto> CreateUserAsync(RegisterDto registerDto);
        Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto updateDto);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> AssignRoleAsync(string userId, string role);
    }
}