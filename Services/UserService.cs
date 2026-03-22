using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.User;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Helpers.Queries;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<UserDto>> GetUsersAsync(UserQueryObject query)
        {
            var usersQuery = _userManager.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Email))
                usersQuery = usersQuery.Where(u =>
                    (u.Email ?? "").ToLower().Contains(query.Email.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.FullName))
                usersQuery = usersQuery.Where(u =>
                    ((u.FirstName ?? "") + " " + (u.LastName ?? ""))
                    .ToLower().Contains(query.FullName.ToLower()));

            usersQuery = query.SortBy?.ToLower() switch
            {
                "email" => query.IsDescending
                    ? usersQuery.OrderByDescending(u => u.Email)
                    : usersQuery.OrderBy(u => u.Email),

                "firstname" => query.IsDescending
                    ? usersQuery.OrderByDescending(u => u.FirstName)
                    : usersQuery.OrderBy(u => u.FirstName),

                _ => usersQuery.OrderByDescending(u => u.CreatedOn)
            };

            var users = await usersQuery
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            // Get roles for each user
            var result = new List<UserDto>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                result.Add(user.ToUserDto(roles.ToList()));
            }

            return result;
        }

        public async Task<UserDto?> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            return user.ToUserDto(roles.ToList());
        }

        public async Task<UserDto> CreateUserAsync(RegisterDto dto)
        {
            // Check if email already exists
            var existing = await _userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                throw new ApplicationException("A user with this email already exists.");

            var user = new AppUser
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Email = dto.Email,
                // Admin-created users have email confirmed automatically
                EmailConfirmed = true,
                CreatedOn = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException(errors);
            }

            await _userManager.AddToRoleAsync(user, "Member");

            var roles = await _userManager.GetRolesAsync(user);
            return user.ToUserDto(roles.ToList());
        }

        public async Task<UserDto?> UpdateUserAsync(string userId, UpdateUserDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            user.ApplyUpdate(dto);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException(errors);
            }

            var roles = await _userManager.GetRolesAsync(user);
            return user.ToUserDto(roles.ToList());
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> AssignRoleAsync(string userId, string role)
        {
            // Only allow valid roles — prevent arbitrary role assignment
            var allowedRoles = new[] { "Admin", "Member", "YouthMember" };

            if (!allowedRoles.Contains(role))
                throw new ApplicationException($"Invalid role. Allowed roles: {string.Join(", ", allowedRoles)}");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            // Remove all current roles then assign the new one
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }
    }
}