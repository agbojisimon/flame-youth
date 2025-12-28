using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.User;
using g_flame_youth.Models;

namespace g_flame_youth.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToUserDto(this AppUser user)
        {
            return new UserDto
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                CreatedOn = user.CreatedOn
            };
        }
        public static AppUser ToAppUser(this RegisterDto registerDto, AppUser user)
        {
            user.FirstName = registerDto.FirstName;
            user.LastName = registerDto.LastName;
            user.UserName = registerDto.UserName;
            user.Email = registerDto.Email;
            return user;
        }
        public static AppUser ToAppUser(this UpdateUserDto dto, AppUser user)
        {
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.UserName = dto.UserName;
            user.Email = dto.Email;
            return user;
        }
    }
}