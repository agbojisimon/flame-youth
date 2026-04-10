namespace GlobalFlameMinistry.API.DTOs.Ministry
{
    public class MinistryResponseDto
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
        public int DisplayOrder { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}