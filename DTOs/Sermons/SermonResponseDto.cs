using GlobalFlameMinistry.API.Models.Enums;

namespace GlobalFlameMinistry.API.DTOs.Sermon
{
    public class SermonResponseDto
    {
        public int Id { get; set; }
        public string Slug { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Speaker { get; set; } = string.Empty;
        public string Series { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? SpeakerImageUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string? Theme { get; set; }
        public string? VideoUrl { get; set; }
        public string? AudioUrl { get; set; }
        public DateTime SermonDate { get; set; }
        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }
        public SermonCategory Category { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}