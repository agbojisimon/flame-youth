using g_flame_youth.DTOs.Account;
using g_flame_youth.Interfaces;
using g_flame_youth.Interfaces.Auth;
using g_flame_youth.Mappers;
using g_flame_youth.Models;
using Microsoft.AspNetCore.Identity;

namespace g_flame_youth.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        public async Task<NewUserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user,
                loginDto.Password,
                false
            );

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = await _tokenService.CreateTokenAsync(user);

            return AuthMapper.ToNewUserDto(user, token);
        }

        public async Task<NewUserDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = AuthMapper.ToAppUser(registerDto);

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new ApplicationException("User creation failed");

            await _userManager.AddToRoleAsync(user, "Member");

            var token = await _tokenService.CreateTokenAsync(user);

            return AuthMapper.ToNewUserDto(user, token);
        }
    }
}