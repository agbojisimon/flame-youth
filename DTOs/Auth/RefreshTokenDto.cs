
using System.ComponentModel.DataAnnotations;
namespace GlobalFlameMinistry.API.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}