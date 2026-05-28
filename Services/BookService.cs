using GlobalFlameMinistry.API.DTOs.Books;
using GlobalFlameMinistry.API.DTOs.Common;
using GlobalFlameMinistry.API.Helpers;
using GlobalFlameMinistry.API.Interfaces;
using GlobalFlameMinistry.API.Mappers;
using Microsoft.Extensions.Caching.Hybrid;

namespace GlobalFlameMinistry.API.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _repository;
        private readonly HybridCache _cache;
        private readonly ILogger<BookService> _logger;

        public BookService(IBookRepository repository, HybridCache cache, ILogger<BookService> logger)
        {
            _repository = repository;
            _cache = cache;
            _logger = logger;
        }

        public async Task<BookResponseDto> CreateAsync(CreateBookDto dto)
        {
            var book = dto.ToModel();
            var created = await _repository.CreateAsync(book);

            try
            {
                await _cache.RemoveByTagAsync(CacheKeys.TagBooks, CancellationToken.None);
                _logger.LogInformation("[BookService] Created book ID {Id}, invalidated cache tag {Tag}", created.Id, CacheKeys.TagBooks);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "[BookService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagBooks);
            }

            return created.ToDto();
        }

        public async Task<BookResponseDto?> GetByIdAsync(int id)
        {
            var cacheKey = string.Format(CacheKeys.BookId, id);

            async ValueTask<BookResponseDto?> Factory(CancellationToken ct)
            {
                var book = await _repository.GetByIdAsync(id);
                return book?.ToDto();
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBooks], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<BookResponseDto?> GetBySlugAsync(string slug)
        {
            var cacheKey = string.Format(CacheKeys.BookSlug, slug);

            async ValueTask<BookResponseDto?> Factory(CancellationToken ct)
            {
                var book = await _repository.GetBySlugAsync(slug);
                return book?.ToDto();
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBooks], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<PagedResult<BookResponseDto>> GetAllAsync(BookQueryObject query)
        {
            var cacheKey = string.Format(CacheKeys.BooksPublished, query.PageNumber, query.PageSize);

            async ValueTask<PagedResult<BookResponseDto>> Factory(CancellationToken ct)
            {
                var books = await _repository.GetAllAsync(query);
                var totalCount = await _repository.GetCountAsync(query);

                return new PagedResult<BookResponseDto>
                {
                    Items = books.ToDtoList(),
                    TotalCount = totalCount,
                    PageNumber = query.PageNumber,
                    PageSize = query.PageSize,
                };
            }

            try
            {
                return await _cache.GetOrCreateAsync(cacheKey, Factory, tags: [CacheKeys.TagBooks], cancellationToken: CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis unavailable, falling back to DB for cache key {CacheKey}", cacheKey);
                return await Factory(CancellationToken.None);
            }
        }

        public async Task<BookResponseDto?> UpdateAsync(int id, UpdateBookDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing is null) return null;

            existing.ApplyUpdate(dto);
            var updated = await _repository.UpdateAsync(id, existing);

            if (updated is not null)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagBooks, CancellationToken.None);
                    _logger.LogInformation("[BookService] Updated book ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBooks);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[BookService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagBooks);
                }
            }

            return updated?.ToDto();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _repository.DeleteAsync(id);

            if (result)
            {
                try
                {
                    await _cache.RemoveByTagAsync(CacheKeys.TagBooks, CancellationToken.None);
                    _logger.LogInformation("[BookService] Deleted book ID {Id}, invalidated cache tag {Tag}", id, CacheKeys.TagBooks);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "[BookService] Redis unavailable, cache invalidation skipped for {Tag}", CacheKeys.TagBooks);
                }
            }

            return result;
        }
    }
}
