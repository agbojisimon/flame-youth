using System.ComponentModel.DataAnnotations;
using GlobalFlameMinistry.API.DTOs.Blog;

namespace GlobalFlameMinistry.API.DTOs.HeroSlide
{
    public class CreateHeroSlideDto : IValidatableObject
    {
        [Required]
        [RegularExpression("^(Image|Video)$",
            ErrorMessage = "MediaType must be 'Image' or 'Video'")]
        public string MediaType { get; set; } = "Image";

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Optional custom slug. When empty, one is generated from <see cref="Title"/>
        /// for video clip slides (with a numeric suffix on collisions).
        /// </summary>
        [StringLength(300)]
        public string? Slug { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        [StringLength(300)]
        public string? Subtitle { get; set; }

        [StringLength(60)]
        public string? Label { get; set; }

        [StringLength(40)]
        public string? ButtonText { get; set; }

        [StringLength(500)]
        public string? TargetUrl { get; set; }

        [Range(0, 10000)]
        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public bool IsFeatured { get; set; } = false;

        public int FeaturedOrder { get; set; } = 0;

        /// <summary>
        /// Structured write-up blocks reusing the blog block model/validation.
        /// Null on an update leaves existing blocks untouched.
        /// </summary>
        public List<CreateBlogBlockDto>? ContentBlocks { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.Equals(MediaType, "Video", StringComparison.OrdinalIgnoreCase)
                && string.IsNullOrWhiteSpace(Title))
            {
                yield return new ValidationResult(
                    "Title is required for video clips.",
                    new[] { nameof(Title) });
            }

            foreach (var result in HeroSlideBlockValidation.ValidateBlocks(ContentBlocks))
            {
                yield return result;
            }
        }
    }
}
