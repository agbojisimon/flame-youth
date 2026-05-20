using GlobalFlameMinistry.API.Data;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Models;
using Microsoft.EntityFrameworkCore;

namespace GlobalFlameMinistry.API.Repositories
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        public BookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Book> CreateAsync(Book book)
        {
            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            // Update slug to include ID
            book.Slug = GlobalFlameMinistry.API.Helpers.SlugHelper.Generate(book.Title, book.Id);
            await _context.SaveChangesAsync();

            return book;
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<Book?> GetBySlugAsync(string slug)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.Slug == slug);
        }

        public async Task<List<Book>> GetAllAsync(BookQueryObject query)
        {
            var books = _context.Books.AsQueryable();

            // Filters 
            if (!string.IsNullOrWhiteSpace(query.Title))
                books = books.Where(b =>
                    b.Title.ToLower().Contains(query.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Author))
                books = books.Where(b =>
                    b.Author.ToLower().Contains(query.Author.ToLower()));

            if (query.IsPublished.HasValue)
                books = books.Where(b => b.IsPublished == query.IsPublished.Value);

            if (query.IsFeatured.HasValue)
                books = books.Where(b => b.IsFeatured == query.IsFeatured.Value);

            // Sorting 
            books = query.SortBy?.ToLower() switch
            {
                "title" => query.IsDescending
                    ? books.OrderByDescending(b => b.Title)
                    : books.OrderBy(b => b.Title),

                "author" => query.IsDescending
                    ? books.OrderByDescending(b => b.Author)
                    : books.OrderBy(b => b.Author),

                "price" => query.IsDescending
                    ? books.OrderByDescending(b => b.Price)
                    : books.OrderBy(b => b.Price),

                "createdon" => query.IsDescending
                    ? books.OrderByDescending(b => b.CreatedOn)
                    : books.OrderBy(b => b.CreatedOn),

                _ => books.OrderByDescending(b => b.CreatedOn)
            };

            // Pagination
            return await books
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(BookQueryObject query)
        {
            var books = _context.Books.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Title))
                books = books.Where(b =>
                    b.Title.ToLower().Contains(query.Title.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.Author))
                books = books.Where(b =>
                    b.Author.ToLower().Contains(query.Author.ToLower()));

            if (query.IsPublished.HasValue)
                books = books.Where(b => b.IsPublished == query.IsPublished.Value);

            if (query.IsFeatured.HasValue)
                books = books.Where(b => b.IsFeatured == query.IsFeatured.Value);

            return await books.CountAsync();
        }

        public async Task<Book?> UpdateAsync(int id, Book book)
        {
            var existing = await _context.Books.FindAsync(id);
            if (existing is null) return null;

            existing.Title = book.Title;
            existing.Author = book.Author;
            existing.Description = book.Description;
            existing.CoverImageUrl = book.CoverImageUrl;
            existing.AmazonUrl = book.AmazonUrl;
            existing.SelarUrl = book.SelarUrl;
            existing.Price = book.Price;
            existing.Currency = book.Currency;
            existing.IsFeatured = book.IsFeatured;
            existing.IsPublished = book.IsPublished;
            existing.UpdatedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Refresh slug in case title changed
            existing.Slug = GlobalFlameMinistry.API.Helpers.SlugHelper.Generate(existing.Title, existing.Id);
            await _context.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book is null) return false;

            // Soft delete
            book.IsDeleted = true;
            book.DeletedOn = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }
    }
}