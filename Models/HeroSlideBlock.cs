namespace GlobalFlameMinistry.API.Models
{
    /// <summary>
    /// A single structured content block (Heading / Paragraph / Image / Quote) that
    /// belongs to a hero slide's write-up. Mirrors <see cref="BlogPostBlock"/> so the
    /// same block model, validation and renderer are reused for clips.
    /// </summary>
    public class HeroSlideBlock
    {
        public int Id { get; set; }
        public int HeroSlideId { get; set; }
        public HeroSlide? HeroSlide { get; set; }
        public string BlockType { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
    }
}
