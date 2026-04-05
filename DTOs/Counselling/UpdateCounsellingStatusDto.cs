using System.ComponentModel.DataAnnotations;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.DTOs.Counselling
{
    public class UpdateCounsellingStatusDto
    {
        [Required]
        public CounsellingStatus Status { get; set; }
    }
}