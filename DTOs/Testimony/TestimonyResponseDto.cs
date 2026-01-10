
namespace g_flame_youth.DTOs.Testimony
{
    public class TestimonyResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Attachment { get; set; }
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}