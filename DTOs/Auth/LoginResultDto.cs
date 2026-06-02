using GlobalFlameMinistry.API.DTOs.Account;

namespace GlobalFlameMinistry.API.DTOs.Auth
{
    public class LoginResultDto
    {
        public NewUserDto? User { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
