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
                Module = dto.Module.Trim(),
                IsPublished = dto.IsPublished,
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
                AuthorId = post.AuthorId,
                AuthorName = post.Author is null
                    ? string.Empty
                    : $"{post.Author.FirstName} {post.Author.LastName}".Trim(),
                Module = post.Module,
                IsPublished = post.IsPublished,
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
    }
}
