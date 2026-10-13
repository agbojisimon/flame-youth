namespace GlobalFlameMinistry.API.Models
{
    public class HeroSlide
    {
        public int Id { get; set; }
        public string MediaType { get; set; } = "Image";
        public string Url { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? TargetUrl { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}