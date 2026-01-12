using g_flame_youth.Models;

namespace g_flame_youth.DTOs.Contact
{
    public class CreateContactDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Message { get; set; } = string.Empty;
        public ContactMessageType Type { get; set; } = ContactMessageType.General;
    }
}