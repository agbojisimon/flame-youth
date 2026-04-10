using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Ministry
{
    public class UpdateMinistryDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(200, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Short description is required")]
        [StringLength(500, MinimumLength = 2)]
        public string ShortDescription { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? LeaderName { get; set; }
        public string? LeaderTitle { get; set; }
        public string? LeaderImageUrl { get; set; }
        public string? ContactEmail { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
    }
}