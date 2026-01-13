using System.ComponentModel.DataAnnotations;

namespace g_flame_youth.DTOs.Account
{
    public class RegisterDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Firstname cannot be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "Firstname must not be greater than 50 characters")]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        [MinLength(3, ErrorMessage = "Lastname cannot be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "Lastname must not be greater than 50 characters")]
        public string LastName { get; set; } = string.Empty;
        [Required]
        [MinLength(3, ErrorMessage = "Username cannot be less than 3 characters")]
        [MaxLength(50, ErrorMessage = "Username must not be greater than 50 characters")]
        public string UserName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}