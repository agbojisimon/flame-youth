namespace GlobalFlameMinistry.API.Models
{
    public class MinistryDepartment
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ShortDescription { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? LeaderName { get; set; }
        public string? LeaderTitle { get; set; }
        public string? LeaderImageUrl { get; set; }
        public string? ContactEmail { get; set; }
        public int DisplayOrder { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}