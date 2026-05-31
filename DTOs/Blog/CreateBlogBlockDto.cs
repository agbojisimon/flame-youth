using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Blog
{
    public class CreateBlogBlockDto : IValidatableObject
    {
        [Required]
        [MaxLength(50)]
        public string BlockType { get; set; } = string.Empty;

        [MaxLength(10000)]
        public string? Content { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public int DisplayOrder { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validTypes = new[] { "Heading", "Paragraph", "Image", "Quote" };

            if (string.IsNullOrWhiteSpace(BlockType) || !validTypes.Contains(BlockType))
            {
                yield return new ValidationResult(
                    "BlockType must be one of: Heading, Paragraph, Image, Quote.",
                    new[] { nameof(BlockType) });
            }

            if (DisplayOrder < 0)
            {
                yield return new ValidationResult(
                    "DisplayOrder must be zero or a positive integer.",
                    new[] { nameof(DisplayOrder) });
            }
        }
    }
}
