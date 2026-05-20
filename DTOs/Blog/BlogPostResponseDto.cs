namespace GlobalFlameMinistry.API.DTOs.Blog
{
    public class BlogPostResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? AuthorId { get; set; }
        public string? AuthorName { get; set; }
        public string Module { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public List<BlogBlockResponseDto> Blocks { get; set; } = new();
    }
}
