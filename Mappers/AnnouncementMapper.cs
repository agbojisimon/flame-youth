using GlobalFlameMinistry.API.DTOs.Announcement;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class AnnouncementMapper
    {
        public static AnnouncementDto ToAnnouncementDto(this Announcement announcement)
        {
            return new AnnouncementDto
            {
                Id = announcement.Id,
                Slug = announcement.Slug,
                Title = announcement.Title,
                Content = announcement.Content,
                CreatedById = announcement.CreatedById,
                Module = announcement.Module,
                Category = announcement.Category,
                IsPublished = announcement.IsPublished,
                CreatedOn = announcement.CreatedOn,
                UpdatedOn = announcement.UpdatedOn
            };
        }
        public static Announcement ToAnnouncementFromCreateDto(this CreateAnnouncementDto createDto, string createdById)
        {
            return new Announcement
            {
                Title = createDto.Title,
                Slug = SlugHelper.Generate(createDto.Title, 0),
                Content = createDto.Content,
                Module = createDto.Module,
                Category = createDto.Category,
                IsPublished = createDto.IsPublished,
                CreatedById = createdById,
                CreatedOn = DateTime.UtcNow
            };
        }
        public static void ApplyUpdate(this Announcement announcement, UpdateAnnouncementDto updateDto)
        {
            announcement.Title = updateDto.Title;
            announcement.Content = updateDto.Content;
            announcement.Category = updateDto.Category;
            announcement.IsPublished = updateDto.IsPublished;
            announcement.UpdatedOn = DateTime.UtcNow;
        }
        public static List<AnnouncementDto> ToDtoList(this IEnumerable<Announcement> announcements)
        {
            return announcements.Select(a => a.ToAnnouncementDto()).ToList();
        }
    }
}