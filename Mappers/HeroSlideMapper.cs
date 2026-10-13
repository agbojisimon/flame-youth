using GlobalFlameMinistry.API.DTOs.HeroSlide;
using GlobalFlameMinistry.API.Models;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class HeroSlideMapper
    {
        public static HeroSlideDto ToHeroSlideDto(this HeroSlide slide)
        {
            return new HeroSlideDto
            {
                Id = slide.Id,
                MediaType = slide.MediaType,
                Url = slide.Url,
                Title = slide.Title,
                Subtitle = slide.Subtitle,
                TargetUrl = slide.TargetUrl,
                DisplayOrder = slide.DisplayOrder,
                IsActive = slide.IsActive,
                CreatedOn = slide.CreatedOn,
                UpdatedOn = slide.UpdatedOn
            };
        }

        public static HeroSlide ToModel(this CreateHeroSlideDto dto)
        {
            return new HeroSlide
            {
                MediaType = dto.MediaType,
                Url = dto.Url,
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                TargetUrl = dto.TargetUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedOn = DateTime.UtcNow
            };
        }

        public static void ApplyUpdate(this HeroSlide slide, UpdateHeroSlideDto dto)
        {
            slide.MediaType = dto.MediaType;
            slide.Url = dto.Url;
            slide.Title = dto.Title;
            slide.Subtitle = dto.Subtitle;
            slide.TargetUrl = dto.TargetUrl;
            slide.DisplayOrder = dto.DisplayOrder;
            slide.IsActive = dto.IsActive;
            slide.UpdatedOn = DateTime.UtcNow;
        }

        public static List<HeroSlideDto> ToHeroSlideDtoList(this IEnumerable<HeroSlide> slides)
        {
            return slides.Select(s => s.ToHeroSlideDto()).ToList();
        }
    }
}