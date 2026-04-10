namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class MyRegistrationDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string EventLocation { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string? EventImageUrl { get; set; }
        public string EventModule { get; set; } = string.Empty;
        public bool EventIsCancelled { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}