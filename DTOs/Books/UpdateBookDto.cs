using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Books
{
    public class UpdateBookDto
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 2)]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Author is required")]
        [StringLength(150)]
        public string Author { get; set; } = string.Empty;
        [StringLength(5000)]
        public string? Description { get; set; }
        [StringLength(500)]
        public string? CoverImageUrl { get; set; }
        [Url(ErrorMessage = "Please provide a valid Amazon URL")]
        [StringLength(500)]
        public string? AmazonUrl { get; set; }
        [Url(ErrorMessage = "Please provide a valid Selar URL")]
        [StringLength(500)]
        public string? SelarUrl { get; set; }
        [Range(0, double.MaxValue)]
        public decimal? Price { get; set; }
        [StringLength(10)]
        public string Currency { get; set; } = "NGN";
        public bool IsFeatured { get; set; } = false;
        public bool IsPublished { get; set; } = false;
    }
}