namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class NewUserDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsYouthMember { get; set; }
        public string? Module { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}