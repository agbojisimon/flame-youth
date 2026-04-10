using GlobalFlameMinistry.API.DTOs.PrayerRequest;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class PrayerRequestMapper
    {
        public static PrayerRequestResponseDto ToPrayerResponseDto(this PrayerRequest prayerModel)
        {
            return new PrayerRequestResponseDto
            {
                Id = prayerModel.Id,
                Name = prayerModel.Name,
                Email = prayerModel.Email,
                Topic = prayerModel.Topic,
                PhoneNumber = prayerModel.PhoneNumber,
                PreferredContact = prayerModel.PreferredContact,
                Content = prayerModel.Content,
                Attachment = prayerModel.Attachment,
                AnonymousToken = prayerModel.AnonymousToken,
                IsAttendedTo = prayerModel.IsAttendedTo,
                AppUserId = prayerModel.AppUserId,
                CreatedAt = prayerModel.CreatedAt
            };
        }

        public static PrayerRequest ToPrayerFromCreateDto(
            this CreatePrayerDto createDto,
            string name,
            string email,
            string? appUserId)
        {
            return new PrayerRequest
            {
                Name = name,
                Email = email,
                PhoneNumber = createDto.PhoneNumber,
                PreferredContact = createDto.PreferredContact,
                Topic = createDto.Topic,
                Content = createDto.Content,
                Attachment = createDto.Attachment,
                AppUserId = appUserId,
                AnonymousToken = Guid.NewGuid().ToString(),
                IsAttendedTo = false,
                CreatedAt = DateTime.UtcNow
            };
        }

        public static List<PrayerRequestResponseDto> ToDtoList(
            this IEnumerable<PrayerRequest> requests)
        {
            return requests.Select(r => r.ToPrayerResponseDto()).ToList();
        }
    }
}