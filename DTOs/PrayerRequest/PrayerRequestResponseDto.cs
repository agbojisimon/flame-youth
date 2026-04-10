namespace GlobalFlameMinistry.API.DTOs.PrayerRequest
{
    public class PrayerRequestResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string PreferredContact { get; set; } = "Email";
        public string? Topic { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Attachment { get; set; }
        public string AnonymousToken { get; set; } = string.Empty;
        public bool IsAttendedTo { get; set; }
        public string? AppUserId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}