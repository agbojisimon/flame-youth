using GlobalFlameMinistry.API.DTOs.Sermon;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class SermonMapper
    {
        public static SermonResponseDto ToDto(this Sermon sermon)
        {
            return new SermonResponseDto
            {
                Id = sermon.Id,
                Title = sermon.Title,
                Speaker = sermon.Speaker,
                Series = sermon.Series,
                Description = sermon.Description,
                SpeakerImageUrl = sermon.SpeakerImageUrl,
                ImageUrl = sermon.ImageUrl,
                VideoUrl = sermon.VideoUrl,
                AudioUrl = sermon.AudioUrl,
                SermonDate = sermon.SermonDate,
                IsPublished = sermon.IsPublished,
                CreatedOn = sermon.CreatedOn,
                UpdatedOn = sermon.UpdatedOn
            };
        }

        public static Sermon ToModel(this CreateSermonDto dto)
        {
            return new Sermon
            {
                Title = dto.Title,
                Speaker = dto.Speaker,
                Series = dto.Series,
                Description = dto.Description,
                SpeakerImageUrl = dto.SpeakerImageUrl,
                ImageUrl = dto.ImageUrl,
                VideoUrl = dto.VideoUrl,
                AudioUrl = dto.AudioUrl,
                SermonDate = dto.SermonDate,
                IsPublished = dto.IsPublished,
                CreatedOn = DateTime.UtcNow
            };
        }

        public static void ApplyUpdate(this Sermon sermon, UpdateSermonDto dto)
        {
            sermon.Title = dto.Title;
            sermon.Speaker = dto.Speaker;
            sermon.Series = dto.Series;
            sermon.Description = dto.Description;
            sermon.SpeakerImageUrl = dto.SpeakerImageUrl;
            sermon.ImageUrl = dto.ImageUrl;
            sermon.VideoUrl = dto.VideoUrl;
            sermon.AudioUrl = dto.AudioUrl;
            sermon.SermonDate = dto.SermonDate;
            sermon.IsPublished = dto.IsPublished;
            sermon.UpdatedOn = DateTime.UtcNow;
        }

        public static List<SermonResponseDto> ToDtoList(
            this IEnumerable<Sermon> sermons)
            => sermons.Select(s => s.ToDto()).ToList();
    }
}