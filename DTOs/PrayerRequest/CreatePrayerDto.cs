using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace g_flame_youth.DTOs.PrayerRequest
{
    public class CreatePrayerDto
    {
        public string Content { get; set; } = string.Empty;
        public string? Attachment { get; set; }
    }
}