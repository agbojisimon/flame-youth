using GlobalFlameMinistry.API.DTOs.Blog;
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
                Slug = slide.Slug,
                Title = slide.Title,
                Subtitle = slide.Subtitle,
                Label = slide.Label,
                ButtonText = slide.ButtonText,
                TargetUrl = slide.TargetUrl,
                DisplayOrder = slide.DisplayOrder,
                IsActive = slide.IsActive,
                IsFeatured = slide.IsFeatured,
                FeaturedOrder = slide.FeaturedOrder,
                CreatedOn = slide.CreatedOn,
                UpdatedOn = slide.UpdatedOn,
                HasWriteUp = slide.ContentBlocks is { Count: > 0 }
            };
        }

        /// <summary>Full clip payload including the ordered write-up blocks.</summary>
        public static HeroSlideDto ToHeroSlideDetailDto(this HeroSlide slide)
        {
            var dto = slide.ToHeroSlideDto();

            dto.ContentBlocks = slide.ContentBlocks
                .OrderBy(b => b.DisplayOrder)
                .Select(b => new BlogBlockResponseDto
                {
                    Id = b.Id,
                    BlockType = b.BlockType,
                    Content = b.Content,
                    ImageUrl = b.ImageUrl,
                    DisplayOrder = b.DisplayOrder
                })
                .ToList();

            return dto;
        }

        public static HeroSlide ToModel(this CreateHeroSlideDto dto)
        {
            return new HeroSlide
            {
                MediaType = dto.MediaType,
                Url = dto.Url,
                Slug = string.IsNullOrWhiteSpace(dto.Slug) ? null : dto.Slug.Trim(),
                Title = dto.Title,
                Subtitle = dto.Subtitle,
                Label = dto.Label,
                ButtonText = dto.ButtonText,
                TargetUrl = dto.TargetUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                IsFeatured = dto.IsFeatured,
                FeaturedOrder = dto.FeaturedOrder,
                CreatedOn = DateTime.UtcNow,
                ContentBlocks = dto.ContentBlocks.ToHeroSlideBlocks()
            };
        }

        public static List<HeroSlideBlock> ToHeroSlideBlocks(this IEnumerable<CreateBlogBlockDto>? blocks)
        {
            if (blocks is null)
                return new List<HeroSlideBlock>();

            return blocks
                .Select(b => new HeroSlideBlock
                {
                    BlockType = b.BlockType,
                    Content = b.Content,
                    ImageUrl = b.ImageUrl,
                    DisplayOrder = b.DisplayOrder
                })
                .ToList();
        }

        public static void ApplyUpdate(this HeroSlide slide, UpdateHeroSlideDto dto)
        {
            slide.MediaType = dto.MediaType;
            slide.Url = dto.Url;
            slide.Title = dto.Title;
            slide.Subtitle = dto.Subtitle;
            slide.Label = dto.Label;
            slide.ButtonText = dto.ButtonText;
            slide.TargetUrl = dto.TargetUrl;
            slide.DisplayOrder = dto.DisplayOrder;
            slide.IsActive = dto.IsActive;
            slide.IsFeatured = dto.IsFeatured;
            slide.FeaturedOrder = dto.FeaturedOrder;
            slide.UpdatedOn = DateTime.UtcNow;
        }

        public static List<HeroSlideDto> ToHeroSlideDtoList(this IEnumerable<HeroSlide> slides)
        {
            return slides.Select(s => s.ToHeroSlideDto()).ToList();
        }
    }
}
