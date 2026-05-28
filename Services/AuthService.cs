using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.Auth;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Interfaces.Auth;
using GlobalFlameMinistry.API.Interfaces.Email;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GlobalFlameMinistry.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IEmailSender emailSender,
            ITokenService tokenService,
            IConfiguration config, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _tokenService = tokenService;
            _config = config;
            _logger = logger;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new ApplicationException("An account with this email already exists.");

            var user = dto.ToAppUser();
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new ApplicationException(errors);
            }

            await _userManager.AddToRoleAsync(user, "Member");

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(emailToken);

            var backendUrl = _config["App:BackendUrl"];
            var confirmationLink =
                $"{backendUrl}/api/auth/confirm-email?email={user.Email}&token={encodedToken}";

            var body = BuildConfirmationEmailBody(user.FirstName, confirmationLink);

            // Attempt to send — track whether it succeeded
            var emailSent = true;
            try
            {
                await _emailSender.SendEmailAsync(
                    user.Email!,
                    "Confirm your email — Global Flame Ministry",
                    body);
            }
            catch (Exception ex)
            {
                emailSent = false;
                _logger.LogError(ex,
                    "[AuthService] Confirmation email failed for {Email} after registration.", user.Email);
            }

            // Account was created — return appropriate message
            return emailSent
                ? "Registration successful. Please check your email to confirm your account."
                : "Your account was created, but we could not send your confirmation email. " +
                  "Please use the 'Resend Confirmation' option to try again.";
        }

        public async Task<NewUserDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email!);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials.");

            if (!user.EmailConfirmed)
                throw new UnauthorizedAccessException(
                    "Please confirm your email before logging in.");

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, dto.Password!, false);

            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials.");

            var token = await _tokenService.CreateTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return user.ToNewUserDto(token, user.RefreshToken!, roles.ToList());
        }

        public async Task<string> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email!);

            if (user == null ||
                user.RefreshToken != dto.RefreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new UnauthorizedAccessException(
                    "Invalid or expired refresh token.");

            var newJwt = await _tokenService.CreateTokenAsync(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return newJwt;
        }

        public async Task<string> ConfirmEmailAsync(EmailConfirmationDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid request.");

            if (user.EmailConfirmed)
                return "Email already confirmed. You can login.";

            var decodedToken = Uri.UnescapeDataString(dto.Token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(e => e.Description));
                throw new ApplicationException(
                    $"Email confirmation failed: {errors}");
            }

            return "Email confirmed successfully. You can now login.";
        }

        public async Task<string> ResendConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return "If the email exists, a confirmation link has been sent.";

            if (user.EmailConfirmed)
                return "Email already confirmed. You can login.";

            var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(emailToken);

            var backendUrl = _config["App:BackendUrl"];
            var link = $"{backendUrl}/api/auth/confirm-email?email={user.Email}&token={encodedToken}";

            var body = BuildResendConfirmationEmailBody(user.FirstName, link);

            try
            {
                await _emailSender.SendEmailAsync(
                    user.Email!,
                    "Confirm your email — Global Flame Ministry",
                    body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[AuthService] Resend confirmation email failed for {Email}", user.Email);
                return "We could not send the confirmation email. Please try again later.";
            }

            return "If the email exists, a confirmation link has been sent.";
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !user.EmailConfirmed) return;

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var frontendUrl = _config["App:FrontendUrl"];
            var resetLink = $"{frontendUrl}/reset-password?email={dto.Email}&token={encodedToken}";

            var body = BuildForgotPasswordEmailBody(user.FirstName, resetLink);

            try
            {
                await _emailSender.SendEmailAsync(
                    dto.Email,
                    "Reset your password — Global Flame Ministry",
                    body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "[AuthService] Forgot-password email failed for {Email}", dto.Email);
            }
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email!);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid reset request.");

            var decodedToken = Uri.UnescapeDataString(dto.Token!);
            var result = await _userManager.ResetPasswordAsync(
                user, decodedToken, dto.NewPassword!);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ",
                    result.Errors.Select(e => e.Description));
                throw new ApplicationException($"Password reset failed: {errors}");
            }

            return "Password reset successful. You can now login with your new password.";
        }

        private static string BuildConfirmationEmailBody(string firstName, string confirmationLink)
        {
            var safeName = (firstName ?? "Beloved").Trim();
            return $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;'>
          <div style='background:linear-gradient(135deg,#7c3aed,#a21caf);padding:32px;
                      text-align:center;border-radius:12px 12px 0 0;'>
            <h1 style='color:white;margin:0;'>Global Flame</h1>
            <p style='color:#ddd6fe;margin:8px 0 0;'>Email Confirmation</p>
          </div>
          <div style='padding:32px;background:white;'>
            <p style='font-size:16px;color:#333;'>Hi <strong>{safeName}</strong>,</p>
            <p style='color:#555;line-height:1.6;'>
              Thank you for joining Global Flame Ministry!
              Please confirm your email address to activate your account.
            </p>
            <div style='text-align:center;margin:32px 0;'>
              <a href='{confirmationLink}'
                style='padding:14px 32px;background:#a21caf;color:white;
                       text-decoration:none;border-radius:8px;font-weight:bold;
                       font-size:16px;'>
                Confirm Email Address
              </a>
            </div>
            <p style='color:#888;font-size:13px;'>
                If you did not register, please ignore this email.
            </p>
            <p style='color:#888;font-size:13px;'>
                Please do not reply to this email. If you need assistance, contact us at
                <a href='mailto:info@globalflameministry.org' style='color:#a21caf;'>
                    info@globalflameministry.org
                </a>
            </p>
          </div>
          <div style='padding:16px;background:#f5f5f5;text-align:center;
                      border-radius:0 0 12px 12px;'>
            <p style='color:#aaa;font-size:12px;margin:0;'>
              © {DateTime.UtcNow.Year} Global Flame Ministry. All rights reserved.
            </p>
          </div>
        </div>";
        }

        private static string BuildResendConfirmationEmailBody(string firstName, string confirmationLink)
        {
            var safeName = (firstName ?? "Beloved").Trim();
            return $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;'>
          <div style='background:linear-gradient(135deg,#7c3aed,#a21caf);padding:32px;
                      text-align:center;border-radius:12px 12px 0 0;'>
            <h1 style='color:white;margin:0;'>Global Flame</h1>
            <p style='color:#ddd6fe;margin:8px 0 0;'>Email Confirmation</p>
          </div>
          <div style='padding:32px;background:white;'>
            <p style='font-size:16px;color:#333;'>Hi <strong>{safeName}</strong>,</p>
            <p style='color:#555;line-height:1.6;'>
              Here is your new confirmation link. Click below to activate your account:
            </p>
            <div style='text-align:center;margin:32px 0;'>
              <a href='{confirmationLink}'
                style='padding:14px 32px;background:#a21caf;color:white;
                       text-decoration:none;border-radius:8px;font-weight:bold;
                       font-size:16px;'>
                Confirm Email Address
              </a>
            </div>
            <p style='color:#888;font-size:13px;'>
              If you did not request this, please ignore this email.
            </p>

            <p style='color:#888;font-size:13px;'>
                Please do not reply to this email. If you need assistance, contact us at
                <a href='mailto:info@globalflameministry.org' style='color:#a21caf;'>
                    info@globalflameministry.org
                </a>
            </p>
          </div>
          <div style='padding:16px;background:#f5f5f5;text-align:center;
                      border-radius:0 0 12px 12px;'>
            <p style='color:#aaa;font-size:12px;margin:0;'>
              © {DateTime.UtcNow.Year} Global Flame Ministry. All rights reserved.
            </p>
          </div>
        </div>";
        }

        private static string BuildForgotPasswordEmailBody(string firstName, string resetLink)
        {
            var safeName = (firstName ?? "Beloved").Trim();
            return $@"
        <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;'>
          <div style='background:linear-gradient(135deg,#7c3aed,#a21caf);padding:32px;
                      text-align:center;border-radius:12px 12px 0 0;'>
            <h1 style='color:white;margin:0;'>Global Flame</h1>
            <p style='color:#ddd6fe;margin:8px 0 0;'>Password Reset</p>
          </div>
          <div style='padding:32px;background:white;'>
            <p style='font-size:16px;color:#333;'>Hi <strong>{safeName}</strong>,</p>
            <p style='color:#555;line-height:1.6;'>
              We received a request to reset your password.
              Click the button below to proceed:
            </p>
            <div style='text-align:center;margin:32px 0;'>
              <a href='{resetLink}'
                style='padding:14px 32px;background:#a21caf;color:white;
                       text-decoration:none;border-radius:8px;font-weight:bold;
                       font-size:16px;'>
                Reset Password
              </a>
            </div>
            <p style='color:#888;font-size:13px;'>
              This link expires in 1 hour.
              If you did not request this, ignore this email.
            </p>

            <p style='color:#888;font-size:13px;'>
                Please do not reply to this email. If you need assistance, contact us at
                <a href='mailto:info@globalflameministry.org' style='color:#a21caf;'>
                    info@globalflameministry.org
                </a>
            </p>
          </div>
          <div style='padding:16px;background:#f5f5f5;text-align:center;
                      border-radius:0 0 12px 12px;'>
            <p style='color:#aaa;font-size:12px;margin:0;'>
              © {DateTime.UtcNow.Year} Global Flame Ministry. All rights reserved.
            </p>
          </div>
        </div>";
        }

        private static string GenerateRefreshToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}