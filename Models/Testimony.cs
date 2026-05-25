namespace GlobalFlameMinistry.API.Models
{
    public class Testimony
    {
        public int Id { get; set; }
        public string? AppUserId { get; set; }
        public AppUser? User { get; set; }
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? Attachment { get; set; }
        public TestimonyStatus Status { get; set; } = TestimonyStatus.Pending;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}