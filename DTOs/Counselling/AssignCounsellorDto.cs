using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Counselling
{
    public class AssignCounsellorDto
    {
        [Required]
        [StringLength(200)]
        public string AssignedTo { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string AssignedToEmail { get; set; } = string.Empty;
    }
}