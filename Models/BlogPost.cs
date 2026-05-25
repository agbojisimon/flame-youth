using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Excerpt { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? AuthorId { get; set; }
        public AppUser? Author { get; set; }
        public string Department { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        public List<BlogPostBlock> Blocks { get; set; } = new();
    }
}
