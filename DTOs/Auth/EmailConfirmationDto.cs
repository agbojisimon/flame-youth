using System.ComponentModel.DataAnnotations;
namespace GlobalFlameMinistry.API.DTOs.Auth
{
    public class EmailConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Token { get; set; } = string.Empty;
    }
}