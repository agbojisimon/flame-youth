using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class AuthMapper
    {
        // RegisterDto → AppUser model
        public static AppUser ToAppUser(this RegisterDto dto)
        {
            return new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Email = dto.Email,
                CreatedOn = DateTime.UtcNow
            };
        }

        // AppUser → NewUserDto — used ONLY after login
        // Includes JWT token and refresh token
        public static NewUserDto ToNewUserDto(this AppUser user, string token, string refreshToken, List<string>? roles = null)
        {
            return new NewUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Module = user.Module,
                Roles = roles ?? new List<string>(),
                Token = token,
                RefreshToken = refreshToken
            };
        }
    }
}