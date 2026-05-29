using System.ComponentModel.DataAnnotations;
using GlobalFlameMinistry.API.Models.Enums;

namespace GlobalFlameMinistry.API.DTOs.Sermon
{
    public class CreateSermonDto
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Speaker { get; set; } = string.Empty;
        [Required]
        [StringLength(100)]
        public string Series { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? SpeakerImageUrl { get; set; }
        public string? ImageUrl { get; set; }
        [StringLength(300)]
        public string? Theme { get; set; }
        public string? VideoUrl { get; set; }
        [StringLength(500)]
        public string? AudioUrl { get; set; }
        [Required]
        public DateTime SermonDate { get; set; }
        public bool IsPublished { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public SermonCategory Category { get; set; } = SermonCategory.Conference;
    }
}