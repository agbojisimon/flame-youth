using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Announcement
{
    public class CreateAnnouncementDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(10000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 10000 characters")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Module is required")]
        [RegularExpression("^(Ministry|Youth)$", ErrorMessage = "Module must be either 'Ministry' or 'Youth'")]
        public string Module { get; set; } = "Ministry";

        [StringLength(50, ErrorMessage = "Category cannot exceed 50 characters")]
        public string Category { get; set; } = string.Empty;
        public bool IsPublished { get; set; } = false;
    }
}