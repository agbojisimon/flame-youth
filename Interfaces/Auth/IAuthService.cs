using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.Auth;

namespace g_flame_youth.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<NewUserDto> RegisterAsync(RegisterDto dto);
        Task<NewUserDto> LoginAsync(LoginDto dto);
        Task<string> RefreshTokenAsync(RefreshTokenDto dto);
        Task<string> ConfirmEmailAsync(EmailConfirmationDto dto);
        Task<string> ResendConfirmationAsync(string email);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task<string> ResetPasswordAsync(ResetPasswordDto dto);
    }
}