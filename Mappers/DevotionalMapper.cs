using g_flame_youth.DTOs.Devotional;
using g_flame_youth.Models;

namespace g_flame_youth.Mappers
{
    public static class DevotionalMapper
    {
        public static DevotionalResponseDto ToDevotionalResponseDto(this Devotional devotional)
        {
            return new DevotionalResponseDto
            {
                Id = devotional.Id,
                Title = devotional.Title,
                Content = devotional.Content,
                DevotionalDate = devotional.DevotionalDate,
                IsPublished = devotional.IsPublished,
                CreatedAt = devotional.CreatedAt,
            };
        }

        public static Devotional ToDevotionalFromCreateDto(this CreateDevotionalDto createDto)
        {
            return new Devotional
            {
                Title = createDto.Title,
                Content = createDto.Content,
                DevotionalDate = createDto.DevotionalDate,
            };
        }
    }
}