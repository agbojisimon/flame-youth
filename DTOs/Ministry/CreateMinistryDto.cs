using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Ministry
{
    public class CreateMinistryDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Short description is required")]
        [StringLength(500, MinimumLength = 2)]
        public string ShortDescription { get; set; } = string.Empty;
        [StringLength(5000)]
        public string? Description { get; set; }
        [StringLength(500)]
        public string? CoverImageUrl { get; set; }
        [StringLength(200)]
        public string? LeaderName { get; set; }
        [StringLength(200)]
        public string? LeaderTitle { get; set; }
        [StringLength(500)]
        public string? LeaderImageUrl { get; set; }
        [StringLength(256)]
        public string? ContactEmail { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
    }
}