namespace GlobalFlameMinistry.API.Models
{
    public class PrayerRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string Content { get; set; } = string.Empty;
        public string AnonymousToken { get; set; } = Guid.NewGuid().ToString();
        public string? Attachment { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? User { get; set; }
        public bool IsAttendedTo { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}