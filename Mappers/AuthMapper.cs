using g_flame_youth.DTOs.Account;
using g_flame_youth.Models;

namespace g_flame_youth.Mappers
{
    public static class AuthMapper
    {
        public static AppUser ToAppUser(this RegisterDto registerDto)
        {
            return new AppUser
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedOn = DateTime.UtcNow
            };
        }

        public static NewUserDto ToNewUserDto(AppUser user, string token)
        {
            return new NewUserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Token = token
            };
        }
    }
}