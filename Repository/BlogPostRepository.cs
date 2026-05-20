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

            if (!string.IsNullOrWhiteSpace(query.Module))
            {
                posts = posts.Where(p => p.Module == query.Module);
            }

            if (query.IsPublished.HasValue)
            {
                posts = posts.Where(p => p.IsPublished == query.IsPublished.Value);
            }

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var search = query.SearchTerm.Trim().ToLowerInvariant();
                posts = posts.Where(p => p.Title.ToLower().Contains(search)
                    || (!string.IsNullOrWhiteSpace(p.Excerpt) && p.Excerpt.ToLower().Contains(search)));
            }

            posts = query.SortBy?.Trim().ToLowerInvariant() switch
            {
                "title" => query.IsDescending ? posts.OrderByDescending(p => p.Title) : posts.OrderBy(p => p.Title),
                "module" => query.IsDescending ? posts.OrderByDescending(p => p.Module) : posts.OrderBy(p => p.Module),
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

            if (!string.IsNullOrWhiteSpace(query.Module))
            {
                posts = posts.Where(p => p.Module == query.Module);
            }

            if (query.IsPublished.HasValue)
            {
                posts = posts.Where(p => p.IsPublished == query.IsPublished.Value);
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
            existing.Module = dto.Module.Trim();
            existing.IsPublished = dto.IsPublished;
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

            await _context.SaveChangesAsync();
            return true;
        }

        private static string GenerateTemporarySlug()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
