using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace GlobalFlameMinistry.API.Models
{
    public class AppUser : IdentityUser
    {
        // Make these required and default to empty string
        public override string? Email { get; set; } = string.Empty;
        public override string? UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string? ProfilePictureUrl { get; set; }
        public string? Module { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public ICollection<Testimony> Testimonies { get; set; } = new List<Testimony>();
    }
}