namespace GlobalFlameMinistry.API.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? AmazonUrl { get; set; }
        public string? SelarUrl { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; } = "NGN";
        public bool IsFeatured { get; set; } = false;
        public bool IsPublished { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}