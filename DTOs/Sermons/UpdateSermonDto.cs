using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Sermon
{
    public class UpdateSermonDto
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
        public string? VideoUrl { get; set; }
        [StringLength(500)]
        public string? AudioUrl { get; set; }

        [Required]
        public DateTime SermonDate { get; set; }

        public bool IsPublished { get; set; }
        public bool IsFeatured { get; set; }
    }
}