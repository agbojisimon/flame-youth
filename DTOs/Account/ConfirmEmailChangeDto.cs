using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class ConfirmEmailChangeDto
    {
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;
    }
}