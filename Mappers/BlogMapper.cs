using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.Models;
using System.Linq;
using System.Text.RegularExpressions;

namespace GlobalFlameMinistry.API.Mappers
{
    public static class BlogMapper
    {
        public static BlogPost ToModel(this CreateBlogPostDto dto, string authorId)
        {
            return new BlogPost
            {
                Title = dto.Title.Trim(),
                Slug = string.Empty,
                Excerpt = dto.Excerpt?.Trim(),
                CoverImageUrl = dto.CoverImageUrl?.Trim(),
                Department = dto.Department.Trim(),
                VideoUrl = dto.VideoUrl?.Trim(),
                ConferenceTheme = dto.ConferenceTheme?.Trim(),
                ThemeScripture = dto.ThemeScripture?.Trim(),
                EventStartsAt = ToUtc(dto.EventStartsAt),
                EventEndsAt = ToUtc(dto.EventEndsAt),
                IsPublished = dto.IsPublished,
                IsFeaturedOnHero = dto.IsFeaturedOnHero,
                AuthorId = authorId,
                Blocks = dto.Blocks
                    .Select(block => new BlogPostBlock
                    {
                        BlockType = block.BlockType.Trim(),
                        Content = block.Content?.Trim(),
                        ImageUrl = block.ImageUrl?.Trim(),
                        DisplayOrder = block.DisplayOrder
                    })
                    .OrderBy(block => block.DisplayOrder)
                    .ToList()
            };
        }

        public static BlogPostResponseDto ToResponseDto(this BlogPost post)
        {
            return new BlogPostResponseDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Excerpt = post.Excerpt,
                CoverImageUrl = post.CoverImageUrl,
                VideoUrl = post.VideoUrl,
                AuthorId = post.AuthorId,
                AuthorName = post.Author is null
                    ? string.Empty
                    : $"{post.Author.FirstName} {post.Author.LastName}".Trim(),
                Department = post.Department,
                ConferenceTheme = post.ConferenceTheme,
                ThemeScripture = post.ThemeScripture,
                EventStartsAt = AsUtc(post.EventStartsAt),
                EventEndsAt = AsUtc(post.EventEndsAt),
                IsPublished = post.IsPublished,
                IsFeaturedOnHero = post.IsFeaturedOnHero,
                CreatedOn = post.CreatedOn,
                UpdatedOn = post.UpdatedOn,
                Blocks = post.Blocks
                    .OrderBy(block => block.DisplayOrder)
                    .Select(block => block.ToResponseDto())
                    .ToList()
            };
        }

        public static BlogBlockResponseDto ToResponseDto(this BlogPostBlock block)
        {
            return new BlogBlockResponseDto
            {
                Id = block.Id,
                BlockType = block.BlockType,
                Content = block.Content,
                ImageUrl = block.ImageUrl,
                DisplayOrder = block.DisplayOrder
            };
        }

        public static List<BlogPostResponseDto> ToDtoList(this IEnumerable<BlogPost> posts)
        {
            return posts.Select(post => post.ToResponseDto()).ToList();
        }

        private static string CreateSlug(string title)
        {
            var slug = title.ToLowerInvariant();
            slug = Regex.Replace(slug, "[^a-z0-9\\s-]", string.Empty);
            slug = Regex.Replace(slug, "\\s+", "-").Trim('-');
            slug = Regex.Replace(slug, "-+", "-");
            return string.IsNullOrWhiteSpace(slug) ? Guid.NewGuid().ToString("N") : slug;
        }

        public static string GenerateSlug(string title, int id)
        {
            var normalized = CreateSlug(title);
            return id > 0 ? $"{normalized}-{id}" : normalized;
        }

        /// <summary>
        /// Normalises an optional client-supplied timestamp to UTC so Npgsql
        /// accepts it for a <c>timestamp with time zone</c> column. Npgsql
        /// rejects <see cref="DateTimeKind.Unspecified"/> values for timestamptz.
        /// </summary>
        public static DateTime? ToUtc(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return value.Value.Kind switch
            {
                DateTimeKind.Utc => value.Value,
                DateTimeKind.Local => value.Value.ToUniversalTime(),
                _ => DateTime.SpecifyKind(value.Value, DateTimeKind.Utc),
            };
        }

        /// <summary>
        /// Tags a stored (UTC) value as <see cref="DateTimeKind.Utc"/> so the JSON
        /// response carries an explicit offset and clients never misread it as local.
        /// </summary>
        private static DateTime? AsUtc(DateTime? value)
        {
            return value.HasValue
                ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc)
                : null;
        }
    }
}