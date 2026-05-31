using System.ComponentModel.DataAnnotations;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.DTOs.Contact
{
    public class CreateContactDto
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Full name must be between 2 and 150 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address")]
        [StringLength(200, ErrorMessage = "Email cannot exceed 200 characters")]
        public string Email { get; set; } = string.Empty;
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(3000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 3000 characters")]
        public string Message { get; set; } = string.Empty;

        [EnumDataType(typeof(ContactMessageType), ErrorMessage = "Invalid message type")]
        public ContactMessageType Type { get; set; } = ContactMessageType.General;
    }
}