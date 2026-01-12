using g_flame_youth.Models;

namespace g_flame_youth.DTOs.Contact
{
    public class ContactResponseDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? PhoneNumber { get; set; }

        public string Message { get; set; } = string.Empty;

        public ContactMessageType Type { get; set; } = ContactMessageType.General;

        public ContactMessageStatus Status { get; set; } = ContactMessageStatus.New;

        public DateTime CreatedAt { get; set; }
    }
}