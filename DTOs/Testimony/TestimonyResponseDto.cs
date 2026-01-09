using g_flame_youth.DTOs.Account;

namespace g_flame_youth.DTOs.Testimony
{
    public class TestimonyResponseDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public UserInfoDto User { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public string Attachment { get; set; } = string.Empty;
        public bool Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}