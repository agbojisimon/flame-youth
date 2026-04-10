namespace GlobalFlameMinistry.API.DTOs.Account
{
    public class MyPrayerRequestDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsAttendedTo { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}