namespace GlobalFlameMinistry.API.DTOs.BulkEmail
{
    public class BulkEmailResponseDto
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string TargetGroup { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalRecipients { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedByName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}