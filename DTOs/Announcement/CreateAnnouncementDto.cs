using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace g_flame_youth.DTOs.Announcement
{
    public class CreateAnnouncementDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }
}