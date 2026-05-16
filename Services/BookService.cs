using GlobalFlameMinistry.API.DTOs.Books;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Memory;

namespace GlobalFlameMinistry.API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<BookService> _logger;

        // Cache key constants
        private const string CACHE_KEY_PUBLISHED_BOOKS = "books_published";
        private const string CACHE_KEY_ALL_BOOKS = "books_all";
        private const string CACHE_KEY_BOOK_ID = "book_id_{0}";

        // Cache expiration times
        private readonly TimeSpan _listCacheExpiration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _itemCacheExpiration = TimeSpan.FromMinutes(10);

        public BookService(IBookRepository repository, IMemoryCache memoryCache, ILogger<BookService> logger)
        {
            _repository = repository;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new book (admin endpoint). Invalidates book caches.
        /// </summary>
        public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
        {
            var book = dto.ToModel();
            var created = await _repository.CreateAsync(book);

            // Invalidate book caches
            InvalidateBookCache();

            _logger.LogInformation("[BookService] Created new book with ID {Id}, invalidating caches", created.Id);

            return created.ToDto();
        }

        /// <summary>
        /// Gets a book by ID with caching. Cache expires after 10 minutes.
        /// </summary>
        public async Task<BookResponseDto?> GetByIdAsync(int id)
        {
            string cacheKey = string.Format(CACHE_KEY_BOOK_ID, id);

            if (_memoryCache.TryGetValue(cacheKey, out BookResponseDto? cachedBook))
            {
                _logger.LogInformation("[BookService] Cache hit for book ID {Id}", id);
                return cachedBook;
            }

            _logger.LogInformation("[BookService] Cache miss for book ID {Id}", id);

            var book = await _repository.GetByIdAsync(id);
            var result = book?.ToDto();

            if (result != null)
            {
                _memoryCache.Set(cacheKey, result, _itemCacheExpiration);
            }

            return result;
        }

        /// <summary>
        /// Gets all published books with caching. Cache expires after 5 minutes.
        /// Typically used for public-facing book listings.
        /// </summary>
        public async Task<PagedResult<BookResponseDto>> GetAllAsync(BookQueryObject query)
        {
            string cacheKey = CACHE_KEY_PUBLISHED_BOOKS;

            // Attempt to retrieve from cache
            if (_memoryCache.TryGetValue(cacheKey, out PagedResult<BookResponseDto>? cachedResult))
            {
                _logger.LogInformation("[BookService] Cache hit for published books");
                return cachedResult!;
            }

            _logger.LogInformation("[BookService] Cache miss for published books - querying database");

            var books = await _repository.GetAllAsync(query);
            var totalCount = await _repository.GetCountAsync(query);

            var result = new PagedResult<BookResponseDto>
            {
                Items = books.ToDtoList(),
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            };

            // Store in cache with 5-minute expiration
            _memoryCache.Set(cacheKey, result, _listCacheExpiration);

            return result;
        }

        /// <summary>
        /// Updates an existing book (admin endpoint). Invalidates related book caches.
        /// </summary>
        public async Task<BookResponseDto?> UpdateAsync(int id, UpdateBookDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null) return null;

            existing.ApplyUpdate(dto);
            var updated = await _repository.UpdateAsync(id, existing);

            if (updated is not null)
            {
                // Invalidate specific book caches
                InvalidateBookCache(id);
                _logger.LogInformation("[BookService] Updated book ID {Id}, invalidating caches", id);
            }

            return updated?.ToDto();
        }

        /// <summary>
        /// Deletes a book (admin endpoint). Invalidates all book caches.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                // Invalidate all book caches
                InvalidateBookCache();
                _logger.LogInformation("[BookService] Deleted book ID {Id}, invalidating caches", id);
            }

            return result;
        }

        /// <summary>
        /// Invalidates all book-related caches.
        /// </summary>
        private void InvalidateBookCache()
        {
            _memoryCache.Remove(CACHE_KEY_PUBLISHED_BOOKS);
            _memoryCache.Remove(CACHE_KEY_ALL_BOOKS);
            _logger.LogDebug("[BookService] Invalidated all book caches");
        }

        /// <summary>
        /// Invalidates specific book cache by ID.
        /// </summary>
        private void InvalidateBookCache(int id)
        {
            _memoryCache.Remove(CACHE_KEY_PUBLISHED_BOOKS);
            _memoryCache.Remove(CACHE_KEY_ALL_BOOKS);
            _memoryCache.Remove(string.Format(CACHE_KEY_BOOK_ID, id));
            _logger.LogDebug("[BookService] Invalidated book cache for ID {Id}", id);
        }
    }
}