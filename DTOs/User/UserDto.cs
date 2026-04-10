namespace GlobalFlameMinistry.API.DTOs.User
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsYouthMember { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string? Module { get; set; }
        public bool EmailConfirmed { get; set; }
        public DateTime CreatedOn { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}