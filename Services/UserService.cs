using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.User;
using g_flame_youth.Helpers;
using g_flame_youth.Interfaces;
using g_flame_youth.Mappers;
using g_flame_youth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace g_flame_youth.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            var allowedRoles = new[] { "Member", "Admin" };
            if (!allowedRoles.Contains(role))
                return false;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }
        public async Task<UserDto> CreateUserAsync(RegisterDto registerDto)
        {
            var appUser = new AppUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
                Email = registerDto.Email,
                CreatedOn = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (!createResult.Succeeded)
                throw new InvalidOperationException("User creation failed.");

            var roleResult = await _userManager.AddToRoleAsync(appUser, "Member");

            if (!roleResult.Succeeded)
                throw new InvalidOperationException("Role assignment failed.");

            return appUser.ToUserDto();
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<List<UserDto>> GetUsersAsync(UserQueryObject query)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Email))
            {
                usersQuery = usersQuery.Where(u => u.Email.Contains(query.Email));
            }

            if (!string.IsNullOrWhiteSpace(query.FullName))
            {
                usersQuery = usersQuery.Where(u =>
                    (u.FirstName + " " + u.LastName).Contains(query.FullName));
            }

            if (!string.IsNullOrWhiteSpace(query.SortBy) &&
                query.SortBy.Equals("Email", StringComparison.OrdinalIgnoreCase))
            {
                usersQuery = query.IsDescending
                    ? usersQuery.OrderByDescending(u => u.Email)
                    : usersQuery.OrderBy(u => u.Email);
            }
            else
            {
                usersQuery = usersQuery.OrderByDescending(u => u.CreatedOn);
            }

            var skip = (query.PageNumber - 1) * query.PageSize;

            var users = await usersQuery.Skip(skip).Take(query.PageSize).ToListAsync();

            return users.Select(u => u.ToUserDto()).ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return user == null ? null : user.ToUserDto();
        }

        public async Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto updateDto)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            user = updateDto.ToAppUser(user);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                throw new InvalidOperationException("User update failed.");

            if (!string.IsNullOrWhiteSpace(updateDto.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, token, updateDto.Password);

                if (!passwordResult.Succeeded) throw new InvalidOperationException("Password update failed.");
            }

            return user.ToUserDto();
        }
    }
}