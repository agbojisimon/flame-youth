using System.Data;
using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.DTOs.Blog;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repository
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly AppDbContext _context;

        public BlogPostRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BlogPost> CreateAsync(BlogPost post)
        {
            post.Slug = GenerateTemporarySlug();
            await _context.BlogPosts.AddAsync(post);
            await _context.SaveChangesAsync();

            post.Slug = BlogMapper.GenerateSlug(post.Title, post.Id);
            await _context.SaveChangesAsync();

            return post;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);

            if (post is null)
                return false;

            post.IsDeleted = true;
            post.IsFeaturedOnHero = false;
            post.FeaturedOnHeroAt = null;
            post.DeletedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BlogPost>> GetAllAsync(BlogQueryObject query)
        {
            var posts = _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Blocks)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Department))
            {
                posts = posts.Where(p => p.Department == query.Department);
            }

            if (query.IsPublished.HasValue)
            {
                posts = posts.Where(p => p.IsPublished == query.IsPublished.Value);
            }

            if (query.FeaturedOnHero == true)
            {
                posts = posts.Where(p => p.IsPublished && p.IsFeaturedOnHero);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var search = query.SearchTerm.Trim().ToLowerInvariant();
                posts = posts.Where(p => p.Title.ToLower().Contains(search)
                    || (!string.IsNullOrWhiteSpace(p.Excerpt) && p.Excerpt.ToLower().Contains(search)));
            }

            posts = query.FeaturedOnHero == true
                ? posts.OrderByDescending(p => p.FeaturedOnHeroAt)
                : query.SortBy?.Trim().ToLowerInvariant() switch
                {
                    "title" => query.IsDescending ? posts.OrderByDescending(p => p.Title) : posts.OrderBy(p => p.Title),
                    "department" => query.IsDescending ? posts.OrderByDescending(p => p.Department) : posts.OrderBy(p => p.Department),
                    "updatedon" => query.IsDescending ? posts.OrderByDescending(p => p.UpdatedOn) : posts.OrderBy(p => p.UpdatedOn),
                    _ => query.IsDescending ? posts.OrderByDescending(p => p.CreatedOn) : posts.OrderBy(p => p.CreatedOn),
                };

            var result = await posts
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            result.ForEach(post => post.Blocks = post.Blocks.OrderBy(block => block.DisplayOrder).ToList());
            return result;
        }

        public async Task<int> GetCountAsync(BlogQueryObject query)
        {
            var posts = _context.BlogPosts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Department))
            {
                posts = posts.Where(p => p.Department == query.Department);
            }

            if (query.IsPublished.HasValue)
            {
                posts = posts.Where(p => p.IsPublished == query.IsPublished.Value);
            }

            if (query.FeaturedOnHero == true)
            {
                posts = posts.Where(p => p.IsPublished && p.IsFeaturedOnHero);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var search = query.SearchTerm.Trim().ToLowerInvariant();
                posts = posts.Where(p => p.Title.ToLower().Contains(search)
                    || (!string.IsNullOrWhiteSpace(p.Excerpt) && p.Excerpt.ToLower().Contains(search)));
            }

            return await posts.CountAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(int id)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Blocks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post is not null)
            {
                post.Blocks = post.Blocks.OrderBy(block => block.DisplayOrder).ToList();
            }

            return post;
        }

        public async Task<BlogPost?> GetBySlugAsync(string slug)
        {
            var post = await _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Blocks)
                .FirstOrDefaultAsync(p => p.Slug == slug);

            if (post is not null)
            {
                post.Blocks = post.Blocks.OrderBy(block => block.DisplayOrder).ToList();
            }

            return post;
        }

        public async Task<BlogPost?> UpdateAsync(int id, UpdateBlogPostDto dto)
        {
            var existing = await _context.BlogPosts
                .Include(p => p.Blocks)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing is null)
                return null;

            existing.Title = dto.Title.Trim();
            existing.Slug = BlogMapper.GenerateSlug(dto.Title, existing.Id);
            existing.Excerpt = dto.Excerpt?.Trim();
            existing.CoverImageUrl = dto.CoverImageUrl?.Trim();
            existing.Department = dto.Department.Trim();
            existing.VideoUrl = dto.VideoUrl?.Trim();
            existing.IsPublished = dto.IsPublished;
            existing.ConferenceTheme = dto.ConferenceTheme?.Trim();
            existing.ThemeScripture = dto.ThemeScripture?.Trim();

            var wasFeatured = existing.IsFeaturedOnHero;
            if (dto.IsFeaturedOnHero)
            {
                existing.IsFeaturedOnHero = true;
                existing.FeaturedOnHeroAt = wasFeatured
                    ? existing.FeaturedOnHeroAt ?? DateTime.UtcNow
                    : DateTime.UtcNow;
            }
            else
            {
                existing.IsFeaturedOnHero = false;
                existing.FeaturedOnHeroAt = null;
            }

            existing.UpdatedOn = DateTime.UtcNow;

            _context.BlogPostBlocks.RemoveRange(existing.Blocks);

            existing.Blocks = dto.Blocks
                .Select(block => new BlogPostBlock
                {
                    BlockType = block.BlockType.Trim(),
                    Content = block.Content?.Trim(),
                    ImageUrl = block.ImageUrl?.Trim(),
                    DisplayOrder = block.DisplayOrder
                })
                .OrderBy(block => block.DisplayOrder)
                .ToList();

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> TogglePublishAsync(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);

            if (post is null)
                return false;

            post.IsPublished = !post.IsPublished;
            post.UpdatedOn = DateTime.UtcNow;

            if (!post.IsPublished)
            {
                post.IsFeaturedOnHero = false;
                post.FeaturedOnHeroAt = null;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BlogDepartmentDto>> GetDistinctDepartmentsAsync(
            bool publishedOnly = true)
        {
            var query = _context.BlogPosts.AsQueryable();

            if (publishedOnly)
                query = query.Where(p => p.IsPublished && !p.IsDeleted);

            return await query
                .GroupBy(p => p.Department)
                .Select(g => new BlogDepartmentDto { Name = g.Key, Count = g.Count() })
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        public async Task<List<BlogPost>> GetRelatedAsync(
            int currentId, string department, int take)
        {
            if (take <= 0)
                return new List<BlogPost>();

            var baseQuery = _context.BlogPosts
                .Include(p => p.Author)
                .Include(p => p.Blocks)
                .Where(p => p.IsPublished && !p.IsDeleted && p.Id != currentId);

            var related = await baseQuery
                .Where(p => p.Department == department)
                .OrderByDescending(p => p.CreatedOn)
                .Take(take)
                .ToListAsync();

            if (related.Count < take)
            {
                var excludeIds = related.Select(p => p.Id).Append(currentId).ToList();

                var fill = await baseQuery
                    .Where(p => !excludeIds.Contains(p.Id))
                    .OrderByDescending(p => p.CreatedOn)
                    .Take(take - related.Count)
                    .ToListAsync();

                related.AddRange(fill);
            }

            related.ForEach(post => post.Blocks = post.Blocks.OrderBy(block => block.DisplayOrder).ToList());
            return related;
        }
        public async Task<int> CountFeaturedAsync(int? excludeId)
        {
            var query = _context.BlogPosts
                .Where(p => p.IsPublished && p.IsFeaturedOnHero);

            if (excludeId.HasValue)
                query = query.Where(p => p.Id != excludeId.Value);

            return await query.CountAsync();
        }

        public async Task<T> RunInSerializableTransactionAsync<T>(Func<Task<T>> operation)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                var result = await operation();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static string GenerateTemporarySlug()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
