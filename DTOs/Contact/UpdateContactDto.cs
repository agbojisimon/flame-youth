using System.ComponentModel.DataAnnotations;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.DTOs.Contact
{
  public class UpdateContactDto
  {
    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(ContactMessageStatus), ErrorMessage = "Invalid status value")]
    public ContactMessageStatus Status { get; set; }
  }
}