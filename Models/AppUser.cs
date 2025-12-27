using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace g_flame_youth.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}".Trim();
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}