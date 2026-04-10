namespace GlobalFlameMinistry.API.Models
{
    public class PrayerRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Topic { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string PreferredContact { get; set; } = "Email";
        public string AnonymousToken { get; set; } = Guid.NewGuid().ToString();
        public string? Attachment { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? User { get; set; }
        public bool IsAttendedTo { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedOn { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}