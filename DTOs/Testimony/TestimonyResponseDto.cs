namespace GlobalFlameMinistry.API.DTOs.Testimony
{
    public class TestimonyResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Attachment { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}