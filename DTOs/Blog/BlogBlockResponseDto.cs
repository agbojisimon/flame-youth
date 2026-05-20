namespace GlobalFlameMinistry.API.DTOs.Blog
{
    public class BlogBlockResponseDto
    {
        public int Id { get; set; }
        public string BlockType { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
    }
}
