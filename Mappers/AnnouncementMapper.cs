using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                Status = announcementModel.Status,
                CreatedById = announcementModel.CreatedById,
                CreatedByName = announcementModel.CreatedBy?.FullName,
                CreatedOn = announcementModel.CreatedOn,
            };
        }
        public static Announcement ToAnnouncementFromCreateDto(this CreateAnnouncementDto createAnnouncementDto, string createdById)
        {
            return new Announcement
            {
                Title = createAnnouncementDto.Title,
                Content = createAnnouncementDto.Content,
                Status = createAnnouncementDto.Status,
                Category = createAnnouncementDto.Category,
                CreatedById = createdById,
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}