using GlobalFlameMinistry.API.DTOs.Account;
using GlobalFlameMinistry.API.DTOs.Auth;

namespace GlobalFlameMinistry.API.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterDto registerDto);
        Task<NewUserDto> LoginAsync(LoginDto loginDto);
        Task<string> RefreshTokenAsync(RefreshTokenDto dto);
        Task<string> ConfirmEmailAsync(EmailConfirmationDto dto);
        Task<string> ResendConfirmationAsync(string email);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
    }
}