using System.ComponentModel.DataAnnotations;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.DTOs.Testimony
{
  public class UpdateTestimonyDto
  {
    [Required(ErrorMessage = "Status is required")]
    [EnumDataType(typeof(TestimonyStatus), ErrorMessage = "Invalid status value")]
    public TestimonyStatus Status { get; set; }
  }
}