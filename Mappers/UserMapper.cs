using GlobalFlameMinistry.API.DTOs.User;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class UserMapper
    {
        // AppUser → UserDto — used for profile views and admin user management
        public static UserDto ToUserDto(this AppUser user, List<string>? roles = null)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                ProfilePictureUrl = user.ProfilePictureUrl,
                Module = user.Module,
                EmailConfirmed = user.EmailConfirmed,
                CreatedOn = user.CreatedOn,
                Roles = roles ?? new List<string>()
            };
        }

        // Applies UpdateUserDto fields onto existing AppUser
        public static void ApplyUpdate(this AppUser user, UpdateUserDto dto)
        {
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.UserName))
                user.UserName = dto.UserName;

            if (!string.IsNullOrWhiteSpace(dto.ProfilePictureUrl))
                user.ProfilePictureUrl = dto.ProfilePictureUrl;

            if (!string.IsNullOrWhiteSpace(dto.Module))
                user.Module = dto.Module;
        }
    }
}