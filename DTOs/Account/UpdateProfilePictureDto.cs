using System.ComponentModel.DataAnnotations;

namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class UpdateProfilePictureDto
    {
        [Required]
        [Url]
        [StringLength(500)]
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}