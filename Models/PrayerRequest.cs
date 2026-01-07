
namespace g_flame_youth.Models
{
    public class PrayerRequest
    {
        public int id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AnonymousToken { get; set; } = Guid.NewGuid().ToString();
        public string? Attachment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}