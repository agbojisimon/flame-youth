using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class ChangeEmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;

        [Required]
        public string CurrentPassword { get; set; } = string.Empty;
    }
}