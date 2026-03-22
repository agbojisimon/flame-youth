using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.PrayerRequest
{
    public class CreatePrayerDto
    {
        public string? Name { get; set; }

        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please share your prayer request")]
        [StringLength(2000, MinimumLength = 5, ErrorMessage = "Prayer request must be between 5 and 2000 characters")]
        public string Content { get; set; } = string.Empty;
        public string? Attachment { get; set; }
    }
}