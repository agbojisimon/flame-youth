using g_flame_youth.DTOs.Announcement;
using g_flame_youth.Models;

namespace g_flame_youth.Mappers
{
    public static class AnnouncementMapper
    {
        public static AnnouncementDto ToAnnouncementDto(this Announcement announcementModel)
        {
            if (announcementModel == null)
            {
                throw new ArgumentNullException(nameof(announcementModel));
            }
            return new AnnouncementDto
            {
                Id = announcementModel.Id,
                Title = announcementModel.Title,
                Content = announcementModel.Content,
                Category = announcementModel.Category,
                CreatedById = announcementModel.CreatedById,
                CreatedOn = announcementModel.CreatedOn,
            };
        }
        public static Announcement ToAnnouncementFromCreateDto(this CreateAnnouncementDto createAnnouncementDto)
        {
            return new Announcement
            {
                Title = createAnnouncementDto.Title,
                Content = createAnnouncementDto.Content,
                Category = createAnnouncementDto.Category,
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}