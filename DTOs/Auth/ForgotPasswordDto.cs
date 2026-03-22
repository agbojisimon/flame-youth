using System.ComponentModel.DataAnnotations;
namespace GlobalFlameMinistry.API.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email")]
        public string Email { get; set; } = string.Empty;
    }
}