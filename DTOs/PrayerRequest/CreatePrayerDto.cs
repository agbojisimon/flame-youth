using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.PrayerRequest
{
    public class CreatePrayerDto
    {
        [Required(ErrorMessage = "Your name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Your email address is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "Please share your prayer request")]
        [StringLength(5000, MinimumLength = 5, ErrorMessage = "Prayer request must be between 5 and 5000 characters")]
        public string Content { get; set; } = string.Empty;
        [Phone]
        public string? PhoneNumber { get; set; }
        [RegularExpression("^(Email|Phone)$",
            ErrorMessage = "PreferredContact must be 'Email' or 'Phone'")]
        public string PreferredContact { get; set; } = "Email";
        [StringLength(50)]
        public string? Topic { get; set; }
        public string? Attachment { get; set; }
    }
}