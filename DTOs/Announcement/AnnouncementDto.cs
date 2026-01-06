using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace g_flame_youth.DTOs.Announcement
{
    public class AnnouncementDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string CreatedById { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string Category { get; set; } = string.Empty;
    }
}