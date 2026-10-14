using GlobalFlameMinistry.API.DTOs.Blog;

namespace GlobalFlameMinistry.API.DTOs.HeroSlide
{
    public class HeroSlideDto
    {
        public int Id { get; set; }
        public string MediaType { get; set; } = "Image";
        public string Url { get; set; } = string.Empty;
        public string? Slug { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Label { get; set; }
        public string? ButtonText { get; set; }
        public string? TargetUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public int FeaturedOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        /// <summary>True when the clip has a write-up (one or more content blocks).</summary>
        public bool HasWriteUp { get; set; }

        /// <summary>
        /// Populated only by the single-clip (by slug) endpoint; null in list responses.
        /// </summary>
        public List<BlogBlockResponseDto>? ContentBlocks { get; set; }
    }
}