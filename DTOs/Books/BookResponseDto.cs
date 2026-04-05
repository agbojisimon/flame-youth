namespace GlobalFlameMinistry.API.DTOs.Books
{
    public class BookResponseDto
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
        public bool IsFeatured { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}