using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.BulkEmail
{
    public class SendBulkEmailDto
    {
        [Required(ErrorMessage = "Subject is required")]
        [StringLength(300)]
        public string Subject { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        [Required(ErrorMessage = "Message body is required")]
        public string HtmlBody { get; set; } = string.Empty;
        public string TargetGroup { get; set; } = "All";
        public string? CustomEmails { get; set; }
        public DateTime? ScheduledAt { get; set; }
    }
}