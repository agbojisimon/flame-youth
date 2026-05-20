namespace GlobalFlameMinistry.API.Models
{
    public class BlogPostBlock
    {
        public int Id { get; set; }
        public int BlogPostId { get; set; }
        public BlogPost? BlogPost { get; set; }
        public string BlockType { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public int DisplayOrder { get; set; }
    }
}
