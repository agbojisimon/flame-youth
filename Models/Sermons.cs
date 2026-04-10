namespace GlobalFlameMinistry.API.Models
{
    public class Sermon
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Speaker { get; set; } = string.Empty;
        public string Series { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? SpeakerImageUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? AudioUrl { get; set; }
        public DateTime SermonDate { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}