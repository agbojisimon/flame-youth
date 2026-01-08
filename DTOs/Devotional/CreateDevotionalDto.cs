
namespace g_flame_youth.DTOs.Devotional
{
    public class CreateDevotionalDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateOnly DevotionalDate { get; set; }
    }
}