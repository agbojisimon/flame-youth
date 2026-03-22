namespace GlobalFlameMinistry.API.DTOs.Event
{
    public class EventRegistrationResponseDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventTitle { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}