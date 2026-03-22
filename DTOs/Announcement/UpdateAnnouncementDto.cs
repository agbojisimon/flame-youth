using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Announcement
{
    public class UpdateAnnouncementDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 200 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "Content must be between 10 and 5000 characters")]
        public string Content { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
    }
}