using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.User
{
    public class UpdateUserDto
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Last name is required")]
        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;
        [MinLength(3)]
        [MaxLength(50)]
        public string? UserName { get; set; }
        public string? ProfilePictureUrl { get; set; }

        // Admin can assign module when updating a user
        // "Ministry" or "Youth"
        public string? Module { get; set; }
    }
}