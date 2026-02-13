using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.Auth;
using g_flame_youth.Interfaces;
using g_flame_youth.Interfaces.Auth;
using g_flame_youth.Interfaces.Email;
using g_flame_youth.Mappers;
using g_flame_youth.Models;
using Microsoft.AspNetCore.Identity;

namespace g_flame_youth.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailSender emailSender, ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _tokenService = tokenService;
        }

        public async Task<string> ConfirmEmailAsync(EmailConfirmationDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid request");

            // Token coming from URL must be decoded
            var decodedToken = Uri.UnescapeDataString(dto.Token);

            // Confirm email in Identity
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
                throw new ApplicationException("Confirmation failed");

            return "Email confirmed successfully";
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || !user.EmailConfirmed)
                return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Uri.EscapeDataString(token);

            var resetLink = $"https://yourfrontend.com/reset-password?email={dto.Email}&token={encodedToken}";

            var body = $@"<h2>Password Reset</h2>
            <p>Click the link below to reset your password:</p>
            <a href='{resetLink}'>Reset Password</a>";

            await _emailSender.SendEmailAsync(dto.Email, "Reset Your Password", body);
        }

        private string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }


        public async Task<NewUserDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException("Please confirm your email first.");


            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials");

            // Create JWT (short-lived token)
            var token = await _tokenService.CreateTokenAsync(user);

            //Create Refresh Token (long-lived token)
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _userManager.UpdateAsync(user);

            return AuthMapper.ToNewUserDto(user, token);
        }

        public async Task<string> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Invalid refresh token.");

            // Generate new JWT
            var newJwt = await _tokenService.CreateTokenAsync(user);

            // Rotate refresh token
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return newJwt;
        }

        public async Task<NewUserDto> RegisterAsync(RegisterDto registerDto)
        {
            var user = AuthMapper.ToAppUser(registerDto);

            // Create user with hashed password
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                throw new ApplicationException("User creation failed");

            // Assign default role
            await _userManager.AddToRoleAsync(user, "Member");

            // GENERATE JWT FOR LOGIN
            var jwtToken = await _tokenService.CreateTokenAsync(user);

            // GENERATE EMAIL CONFIRMATION TOKEN
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Make email token URL safe
            var encodedToken = Uri.EscapeDataString(emailToken);

            // Confirmation link user will click
            var confirmationLink = $"http://localhost:5020/api/Auth/confirm-email?email={user.Email}&token={encodedToken}";

            // Email HTML
            var body = $@"<h2>Welcome to Global Flame Youth Community</h2>
            <p>Please confirm your email to activate your account:</p>
            <a href='{confirmationLink}'>Confirm Email</a>";

            // Send confirmation email
            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", body);

            // Return user info + JWT (NOT email token)
            return AuthMapper.ToNewUserDto(user, jwtToken);
        }

        public async Task<string> ResendConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return "If the email exists, confirmation mail was sent.";

            if (user.EmailConfirmed)
                return "Email already confirmed.";

            // Generate new confirmation token
            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var encodedToken = Uri.EscapeDataString(emailToken);

            var link = $"https://yourfrontend.com/confirm-email?email={user.Email}&token={encodedToken}";

            var body = $@"<h2>Email Confirmation</h2>
            <p>Click below to confirm your email:</p>
            <a href='{link}'>Confirm Email</a>";

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email", body);

            return "Confirmation email sent.";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid reset request.");

            // Reset password using token + new password
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
                throw new ApplicationException("Password reset failed.");

            return "Password reset successful.";
        }
    }
}