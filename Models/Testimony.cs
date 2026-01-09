
namespace g_flame_youth.Models
{
    public class Testimony
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty;
        public AppUser User { get; set; } = null!;
        public string Content { get; set; } = string.Empty;
        public string Attachment { get; set; } = string.Empty;
        public bool Status { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}