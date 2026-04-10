using GlobalFlameMinistry.API.DTOs.User;
using GlobalFlameMinistry.API.Helpers;

namespace GlobalFlameMinistry.API.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetUsersAsync(UserQueryObject query);
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> AssignRoleAsync(string userId, string role);
    }
}