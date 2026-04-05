using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Counselling
{
    public class CreateCounsellingRequestDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(200, MinimumLength = 2)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Topic is required")]
        [StringLength(200, MinimumLength = 3)]
        public string Topic { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(5000, MinimumLength = 20)]
        public string Message { get; set; } = string.Empty;

        [RegularExpression("^(Email|Phone)$",
            ErrorMessage = "PreferredContact must be 'Email' or 'Phone'")]
        public string PreferredContact { get; set; } = "Email";
    }
}