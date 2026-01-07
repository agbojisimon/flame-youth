using g_flame_youth.Models;

namespace g_flame_youth.DTOs.PrayerRequest
{
    public class PrayerRequestResponseDto
    {
        public int id { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Attachment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}