using GlobalFlameMinistry.API.DTOs.Testimony;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class TestimonyMapper
    {
        public static TestimonyResponseDto ToTestimonyResponseDto(this Testimony testimonyModel)
        {
            return new TestimonyResponseDto
            {
                Id = testimonyModel.Id,
                FullName = testimonyModel.FullName ?? "Anonymous",
                Content = testimonyModel.Content,
                Attachment = testimonyModel.Attachment,
                Status = testimonyModel.Status.ToString(),
                CreatedAt = testimonyModel.CreatedAt,
                UpdatedAt = testimonyModel.UpdatedAt
            };
        }

        public static Testimony ToTestimonyFromCreateDto(this CreateTestimonyDto createDto, string? name, string? appUserId)
        {
            return new Testimony
            {
                FullName = name,
                Content = createDto.Content,
                Attachment = createDto.Attachment,
                AppUserId = appUserId,
                Status = TestimonyStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }
        public static List<TestimonyResponseDto> ToDtoList(this IEnumerable<Testimony> testimonies)
        {
            return testimonies.Select(t => t.ToTestimonyResponseDto()).ToList();
        }
    }
}