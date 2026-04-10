using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class UpdateProfileDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 3)]
        public string? UserName { get; set; }
    }
}