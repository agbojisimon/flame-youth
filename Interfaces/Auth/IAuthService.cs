using g_flame_youth.DTOs.Account;

namespace g_flame_youth.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<NewUserDto> RegisterAsync(RegisterDto dto);
        Task<NewUserDto> LoginAsync(LoginDto dto);
    }
}