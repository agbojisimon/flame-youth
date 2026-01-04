using System.ComponentModel.DataAnnotations;

namespace g_flame_youth.DTOs.Announcement
{
    public class UpdateAnnouncementDto
    {
        [Required]
        [MaxLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        [MinLength(3, ErrorMessage = "Title must be at least 3 characters long.")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [MinLength(10, ErrorMessage = "Content must be at least 10 characters long.")]
        public string Content { get; set; } = string.Empty;
        [Required]
        [MaxLength(50, ErrorMessage = "Category cannot exceed 50 characters.")]
        [MinLength(3, ErrorMessage = "Category must be at least 3 characters long.")]
        public string Category { get; set; } = string.Empty;
    }
}