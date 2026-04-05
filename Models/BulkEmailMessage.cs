namespace GlobalFlameMinistry.API.Models
{
    public class BulkEmailMessage
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string TargetGroup { get; set; } = "All";
        public string? CustomEmailsJson { get; set; }
        public string Status { get; set; } = "Scheduled";
        public int TotalRecipients { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public DateTime? ScheduledAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string? CreatedByUserId { get; set; }
        public string? CreatedByName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}