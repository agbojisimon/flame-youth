using g_flame_youth.DTOs.PrayerRequest;
using g_flame_youth.Models;

namespace g_flame_youth.Mappers
{
    public static class PrayerRequestMapper
    {
        public static PrayerRequestResponseDto ToPrayerRequestResponseDto(this PrayerRequest prayerModel)
        {
            return new PrayerRequestResponseDto
            {
                id = prayerModel.id,
                Content = prayerModel.Content,
                Attachment = prayerModel.Attachment,
                CreatedAt = prayerModel.CreatedAt
            };
        }

        public static PrayerRequest ToPrayerRequestFromCreateDto(this CreatePrayerDto createDto)
        {
            return new PrayerRequest
            {
                Content = createDto.Content,
                Attachment = createDto.Attachment,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}