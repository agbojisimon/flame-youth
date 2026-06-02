using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.Auth;

namespace GlobalFlameMinistry.API.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<LoginResultDto> LoginAsync(LoginDto loginDto);
        Task<LoginResultDto> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId);
        Task<string> ConfirmEmailAsync(EmailConfirmationDto dto);
        Task<string> ResendConfirmationAsync(string email);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
    }
}