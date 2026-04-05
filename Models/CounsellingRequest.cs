namespace GlobalFlameMinistry.API.Models
{
    public class CounsellingRequest
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string PreferredContact { get; set; } = "Email";
        public string? AssignedTo { get; set; }
        public string? AssignedToEmail { get; set; }
        public CounsellingStatus Status { get; set; } = CounsellingStatus.New;
        public string? AppUserId { get; set; }
        public AppUser? User { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}