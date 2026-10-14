using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.HeroSlide
{
    public class CreateHeroSlideDto
    {
        [Required]
        [RegularExpression("^(Image|Video)$",
            ErrorMessage = "MediaType must be 'Image' or 'Video'")]
        public string MediaType { get; set; } = "Image";

        [Required]
        [StringLength(500)]
        public string Url { get; set; } = string.Empty;

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
    }
}