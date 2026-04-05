namespace GlobalFlameMinistry.API.DTOs.BulkEmail
{
    public class BulkEmailStatsDto
    {
        public int TotalEmailsSent { get; set; }
        public int TotalRecipientsReached { get; set; }
        public int TotalScheduled { get; set; }
        public int TotalFailed { get; set; }
        public double SuccessRate { get; set; }
    }
}