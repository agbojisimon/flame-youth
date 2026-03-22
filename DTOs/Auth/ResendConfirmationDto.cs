using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Auth
{
    public class ResendConfirmationDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}