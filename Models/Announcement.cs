namespace GlobalFlameMinistry.API.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CreatedById { get; set; } = string.Empty;
        public string Module { get; set; } = "Ministry";
        public string Category { get; set; } = string.Empty;
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
    }
}