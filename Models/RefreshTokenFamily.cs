namespace GlobalFlameMinistry.API.Models
{
    public class RefreshTokenFamily
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public Guid FamilyId { get; set; } = Guid.NewGuid();
        public string TokenHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public bool IsInvalidated { get; set; }

        public AppUser User { get; set; } = null!;
    }
}
