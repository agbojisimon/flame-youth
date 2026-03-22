using System.ComponentModel.DataAnnotations;
namespace GlobalFlameMinistry.API.DTOs.Testimony
{
    public class CreateTestimonyDto
    {
        public string? FullName { get; set; }

        [Required(ErrorMessage = "Please share your testimony")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Testimony must be between 10 and 2000 characters")]
        public string Content { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Attachment URL cannot exceed 500 characters")]
        public string? Attachment { get; set; }
    }
}