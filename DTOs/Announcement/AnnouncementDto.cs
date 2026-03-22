namespace GlobalFlameMinistry.API.DTOs.Announcement
{
    public class AnnouncementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? CreatedById { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}