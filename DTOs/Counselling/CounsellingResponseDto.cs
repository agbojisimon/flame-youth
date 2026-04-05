namespace GlobalFlameMinistry.API.DTOs.Counselling
{
    public class CounsellingResponseDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Topic { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string PreferredContact { get; set; } = string.Empty;
        public string? AssignedTo { get; set; }
        public string? AssignedToEmail { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? AppUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}